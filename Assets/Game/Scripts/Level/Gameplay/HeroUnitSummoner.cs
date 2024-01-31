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
	using Sirenix.Utilities;

	public interface IHeroUnitSummoner
	{
		ReactiveCommand SummoningPaidUnit { get; }
		ReactiveCommand<IUnitFacade> SummonedPaidUnit { get; }
		bool TrySummonByCurrency();
		IUnitFacade Summon(Species species, int gradeIndex, int power, Vector2Int position);
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
		Dictionary<Species, int> _speciesCounter = new Dictionary<Species, int>();

		bool _isPaidSummonInterrupted;

		public void Initialize()
		{
			_gameCycle.State
				.Where(state => state == GameState.CompleteWave)
				.Subscribe(_ => OnCompleteWave())
				.AddTo(this);

			InitCounter();
		}

		private void OnCompleteWave()
		{
			var units = _vanishSubscribes.Keys.ToList();

			for (var i = 0; i < units.Count; i++)
				RemoveDeadUnit(units[i]);
		}

		#region IHeroUnitSummoner

		public ReactiveCommand SummoningPaidUnit { get; } = new ReactiveCommand();
		public ReactiveCommand<IUnitFacade> SummonedPaidUnit { get; } = new ReactiveCommand<IUnitFacade>();

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

			SummoningPaidUnit.Execute();

			if (_isPaidSummonInterrupted == false)
				Summon();

			_isPaidSummonInterrupted = false;

			return true;
		}

		public IUnitFacade Summon(Species species, int gradeIndex, int power, Vector2Int position)
		{
			IUnitFacade unit = _unitSpawner.SpawnHeroUnit(species, gradeIndex, power);
			_fieldFacade.AddUnit(unit, position);

			if (_isPaidSummonInterrupted)
				SummonedPaidUnit.Execute(unit);

			SubscribeToUnit(unit);

			return unit;
		}

		public void InterruptPaidSummon() =>
			_isPaidSummonInterrupted = true;

		#endregion

		private void Summon()
		{
			Species summonSpecies = GetRandomSpecies();

			int defaultGradeIndex = 0;
			int defaultPower = _gameUpgrades.GetUnitPower(summonSpecies);

			IUnitFacade unit = _unitSpawner.SpawnHeroUnit(summonSpecies, defaultGradeIndex, defaultPower);
			_fieldFacade.AddUnit(unit);

			SummonedPaidUnit.Execute(unit);

			SubscribeToUnit(unit);
		}

		// TODO: Refact: Optimize for units count > 2
		private Species GetRandomSpecies()
		{
			UpdateCounter();
			int summonedCount = _speciesCounter.Sum(v => v.Value);
			List<Species> species = new List<Species>(_unitsConfig.HeroDefaultSquad);

			_unitsConfig.HeroDefaultSquad.ForEach(s =>
			{
				float speciesRatio = (float)_speciesCounter[s] / summonedCount;

				if (speciesRatio > _unitsConfig.SummonCountRatioLimit)
					species.Remove(s);
			});

			return species[UnityEngine.Random.Range(0, species.Count)];
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

		private void InitCounter() =>
			_unitsConfig.HeroDefaultSquad.ForEach(species => _speciesCounter.Add(species, 0));

		private void UpdateCounter()
		{
			_unitsConfig.HeroDefaultSquad.ForEach(species => _speciesCounter[species] = 0);
			_fieldFacade.Units.ForEach(unit => _speciesCounter[unit.Species]++);
		}
	}
}
