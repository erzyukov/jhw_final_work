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

	public interface IHeroUnitSummoner
	{
		ReactiveCommand SummoningPaidUnit { get; }
		bool TrySummonByCurrency();
		void Summon(Species species, int gradeIndex, Vector2Int position);
		void InterruptPaidSummon();
	}

	public class HeroUnitSummoner : ControllerBase, IHeroUnitSummoner
	{
		[Inject] private CurrencyConfig _currencyConfig;
		[Inject] private IGameCurrency _gameCurrency;
		[Inject] private IUiMessage _haveNeedOfMessage;
		[Inject] private IUnitSpawner _unitSpawner;
		[Inject] private IFieldHeroFacade _fieldFacade;

		Dictionary<IUnitFacade, IDisposable> _unitSubscribes = new Dictionary<IUnitFacade, IDisposable>();
		bool _isPaidSummonInterrupted;

		#region IHeroUnitSummoner

		public ReactiveCommand SummoningPaidUnit { get; } = new ReactiveCommand();

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

		public void Summon(Species species, int gradeIndex, Vector2Int position)
		{
			IUnitFacade unit = _unitSpawner.SpawnHeroUnit(species, gradeIndex);
			_fieldFacade.AddUnit(unit, position);

			SubscribeToUnit(unit);
		}

		public void InterruptPaidSummon() => 
			_isPaidSummonInterrupted = true;

		#endregion

		private void Summon()
		{
			//Species defaultSpecies = Species.HeroInfantryman;
			Species defaultSpecies = Species.HeroSniper;
			int defaultGradeIndex = 0;
			IUnitFacade unit = _unitSpawner.SpawnHeroUnit(defaultSpecies, defaultGradeIndex);
			_fieldFacade.AddUnit(unit);

			SubscribeToUnit(unit);
		}

		private void SubscribeToUnit(IUnitFacade unit)
		{
			_unitSubscribes.Add(unit, default);
			_unitSubscribes[unit] = unit.Died
				.Subscribe(_ => UnitDiedHandler(unit));
		}

		private void UnitDiedHandler(IUnitFacade unit) {}
	}
}
