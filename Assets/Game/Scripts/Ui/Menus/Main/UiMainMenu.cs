namespace Game.Ui
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Core;

	public class UiMainMenu : ControllerBase, IInitializable
	{
		[Inject] private IUiMainMenuView _view;
		[Inject] private IGameCycle _gameCycle;

		public void Initialize()
		{
			_gameCycle.State
				.Subscribe(OnGameStateChanged)
				.AddTo(this);

			foreach (var button in _view.Buttons)
				button.ButtonClicked.Subscribe(_ => OnMenuBattonClicked(button.TargetGameState)).AddTo(this);
		}

		private void OnMenuBattonClicked(GameState state)
		{
			_gameCycle.SetState(state);
		}

		private void OnGameStateChanged(GameState state)
		{
			_view.SetActive(_view.ActiveOnGameState.Contains(state));

            foreach (var button in _view.Buttons)
				_view.SetButtonLocked(button.TargetGameState);

			// TODO: refact [hardcode]: take out to configs
			_view.SetButtonActive(GameState.Lobby);
			_view.SetButtonActive(GameState.Upgrades);
			
			_view.SetButtonSelected(state);
		}
	}
}
