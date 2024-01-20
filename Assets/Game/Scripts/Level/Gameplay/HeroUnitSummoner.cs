namespace Game.Level
{
	using Game.Configs;
	using Game.Core;
	using Game.Field;
	using Game.Ui;
	using Game.Units;
	using Game.Utilities;
	using System;
	using System.Collections.Generic;
	using Zenject;
	using UniRx;
	using UnityEngine;
	using System.Linq;

	public interface IHeroUnitSummoner
	{
		ReactiveCommand<int> SummoningPaidUnit { get; }
		bool TrySummonByCurrency();
		void Summon(Species species, int gradeIndex, int power, Vector2Int position);
		void InterruptPaidSummon();
	}

	public class HeroUnitSummoner : ControllerBase, IHeroUnitSummoner, IInitializable
	{
		[Inject] private CurrencyConfig _currencyConfig;
		[Inject] private UnitsConfig _unitsConfig;
		[Inject] private IGameCurrency _gameCurrency;
		[Inject] private IUiMessage _haveNeedOfMessage;
		[Inject] private IUnitSpawner _unitSpawner;
		[Inject] private IFieldHeroFacade _fieldFacade;
		[Inject] private TimingsConfig _timingsConfig;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IGameUpgrades _gameUpgrades;

		Dictionary<IUnitFacade, IDisposable> _unitDiedSubscribes = new Dictionary<IUnitFacade, IDisposable>();
		Dictionary<IUnitFacade, IDisposable> _vanishSubscribes = new Dictionary<IUnitFacade, IDisposable>();

		bool _isPaidSummonInterrupted;

		public void Initialize()
		{
			_gameCycle.State
				.Where(state => state == GameState.CompleteWave)
				.Subscribe(_ => OnCompleteWave())
				.AddTo(this);
		}

		private void OnCompleteWave()
		{
			var units = _vanishSubscribes.Keys.ToList();
			for (var i = 0; i < units.Count; i++)
				RemoveDeadUnit(units[i]);
		}

		#region IHeroUnitSummoner

		public ReactiveCommand<int> SummoningPaidUnit { get; } = new ReactiveCommand<int>();

		public bool TrySummonByCurrency()
		{
			if (_fieldFacade.HasFreeSpace == false)
			{
				_haveNeedOfMessage.ShowMessage(UiMessage.NotEnoughFreeSpace);
				return false;
			}

			int summonPrice = _currencyConfig.UnitSummonPrice;
			if (_gameCurrency.TrySpendSummonCurrency(summonPrice) == false)
			{
				_haveNeedOfMessage.ShowMessage(UiMessage.NotEnoughSummonCurrency);
				return false;
			}

			SummoningPaidUnit.Execute(summonPrice);

			if (_isPaidSummonInterrupted == false)
				Summon();

			_isPaidSummonInterrupted = false;

			return true;
		}

		public void Summon(Species species, int gradeIndex, int power, Vector2Int position)
		{
			IUnitFacade unit = _unitSpawner.SpawnHeroUnit(species, gradeIndex, power);
			_fieldFacade.AddUnit(unit, position);
			SubscribeToUnit(unit);
		}

		public void InterruptPaidSummon() => 
			_isPaidSummonInterrupted = true;

		#endregion

		private void Summon()
		{
			Species summonSpecies = _unitsConfig.HeroDefaultSquad[UnityEngine.Random.Range(0, _unitsConfig.HeroDefaultSquad.Count)];
			int defaultGradeIndex = 0;
			int defaultPower = _gameUpgrades.GetUnitPower(summonSpecies);

			IUnitFacade unit = _unitSpawner.SpawnHeroUnit(summonSpecies, defaultGradeIndex, defaultPower);
			_fieldFacade.AddUnit(unit);

			SubscribeToUnit(unit);
		}

		private void SubscribeToUnit(IUnitFacade unit)
		{
			_unitDiedSubscribes.Add(unit, default);
			_unitDiedSubscribes[unit] = unit.Events.Died
				.Subscribe(_ => OnUnitDied(unit));
		}

		private void OnUnitDied(IUnitFacade unit)
		{
			IDisposable vanish = Observable.Timer(TimeSpan.FromSeconds(_timingsConfig.UnitDeathVanishDelay))
				.Subscribe(_ => RemoveDeadUnit(unit))
				.AddTo(this);

			_vanishSubscribes.Add(unit, vanish);
		}

		private void RemoveDeadUnit(IUnitFacade unit)
		{
			_vanishSubscribes[unit]?.Dispose();
			_vanishSubscribes.Remove(unit);

			unit.SetActive(false);
		}
	}
}
