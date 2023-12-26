namespace Game.Ui
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Core;
	using Game.Configs;
	using System.Linq;
	using Game.Profiles;

	public class UiMainMenu : ControllerBase, IInitializable
	{
		[Inject] private IUiMainMenuView _view;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private MenuConfig _menuConfig;
		[Inject] private GameProfile _gameProfile;

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
			{
                int availableLevelIndex = _menuConfig.GetAccessLevel(button.TargetGameState) - 1;

				bool isAvailable = 
					availableLevelIndex < _gameProfile.Levels.Count && 
					_gameProfile.Levels[availableLevelIndex].Unlocked.Value;

				if (isAvailable)
					_view.SetButtonActive(button.TargetGameState);
				else
					_view.SetButtonLocked(button.TargetGameState);
			}
			
			_view.SetButtonSelected(state);
		}
	}
}
