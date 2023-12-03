namespace Game.Ui
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Core;
	using Game.Configs;
	using Game.Level;
	using Game.Field;
	using System;

	public class UiTacticalStage : ControllerBase, IInitializable
	{
		[Inject] private IUiTacticalStageHud _hud;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IHeroUnitSummoner _heroUnitSummoner;
		[Inject] private CurrencyConfig _currencyConfig;
		[Inject] private IFieldHeroFacade _fieldHeroFacade;

		public void Initialize()
		{
			_hud.StartBattleButtonClicked
				.Subscribe(_ => _gameCycle.SetState(GameState.BattleStage))
				.AddTo(this);

			_hud.SummonButtonClicked
				.Subscribe(_ => _heroUnitSummoner.TrySummonByCurrency())
				.AddTo(this);
			_hud.SetSummonPrice(_currencyConfig.UnitSummonPrice);

			Observable.Merge(
					_fieldHeroFacade.Events.UnitDragging.Select(_ => false),
					_fieldHeroFacade.Events.UnitDropped.Select(_ => true),
					_fieldHeroFacade.Events.UnitRemoved.Select(_ => true)
				)
				.Subscribe(SetUiActive)
				.AddTo(this);

			_fieldHeroFacade.Units.ObserveCountChanged()
				.StartWith(0)
				.Select(count => count != 0)
				.Subscribe(_hud.SetStartBattleButtonInteractable)
				.AddTo(this);
				
		}

		private void SetUiActive(bool value)
		{
			_hud.SetStartBattleButtonActive(value);
			_hud.SetSummonButtonActive(value);
		}
	}
}