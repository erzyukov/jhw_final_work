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
		void Summon();
	}

	public class HeroUnitSummoner : ControllerBase, IHeroUnitSummoner
	{
		[Inject] private CurrencyConfig _currencyConfig;
		[Inject] private IGameCurrency _gameCurrency;
		[Inject] private IUiHaveNeedOfMessage _haveNeedOfMessage;
		[Inject] private IUnitSpawner _unitSpawner;
		[Inject] private IFieldHeroFacade _fieldFacade;
		[Inject] private IGameCycle _gameCycle;

		Dictionary<IUnitFacade, IDisposable> _unitSubscribes = new Dictionary<IUnitFacade, IDisposable>();

		public void Summon()
		{
			if (_fieldFacade.HasFreeSpace == false)
			{
				// TODO: show message: out of free space
				return;
			}

			int summonPrice = _currencyConfig.UnitSummonPrice;
			if (_gameCurrency.TrySpendSummonCurrency(summonPrice) == false)
			{
				_haveNeedOfMessage.ShowMessage(NeedMessage.SummonCurrency);
				return;
			}

			Species defaultSpecies = Species.HeroInfantryman;
			IUnitFacade unit = _unitSpawner.SpawnHeroUnit(defaultSpecies);
			Vector2Int position = _fieldFacade.AddUnit(unit);

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
