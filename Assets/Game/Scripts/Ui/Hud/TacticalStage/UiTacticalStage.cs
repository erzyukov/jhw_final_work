namespace Game.Ui
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Core;
	using Game.Units;

	public class UiTacticalStage : ControllerBase, IInitializable
	{
		[Inject] private IUiTacticalStageHud _hud;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private UnitFacade.Factory _unitFactory;

		public void Initialize()
		{
			_hud.Opening
				.Subscribe(_ => OnOpeningHandler())
				.AddTo(this);

			_hud.StartBattleButtonClicked
				.Subscribe(_ => _gameCycle.SetState(GameState.BattleStage))
				.AddTo(this);

			_hud.SummonButtonClicked
				.Subscribe(_ => _unitFactory.Create(Species.HeroInfantryman))
				.AddTo(this);
		}

		private void OnOpeningHandler()
		{
			_hud.SetSummonPrice(5);
		}
	}
}
