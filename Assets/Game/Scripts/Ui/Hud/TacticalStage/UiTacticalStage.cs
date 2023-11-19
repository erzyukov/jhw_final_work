namespace Game.Ui
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Core;
	using Game.Configs;
	using Game.Level;

	public class UiTacticalStage : ControllerBase, IInitializable
	{
		[Inject] private IUiTacticalStageHud _hud;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IHeroUnitSummoner _heroUnitSummoner;
		[Inject] private CurrencyConfig _currencyConfig;

		public void Initialize()
		{
			_hud.StartBattleButtonClicked
				.Subscribe(_ => _gameCycle.SetState(GameState.BattleStage))
				.AddTo(this);

			_hud.SummonButtonClicked
				.Subscribe(_ => _heroUnitSummoner.Summon())
				.AddTo(this);
			_hud.SetSummonPrice(_currencyConfig.UnitSummonPrice);
		}
	}
}