namespace Game.Ui
{
	using Game.Core;
	using Game.Utilities;
	using System.Collections.Generic;
	using UniRx;
	using Zenject;

	public interface IScreenNavigator
	{
		ReadOnlyReactiveProperty<Screen> Screen { get; }
		void Open(Screen screen);
		void CloseActive();
	}

	public class ScreenNavigator : ControllerBase, IScreenNavigator, IInitializable
	{
		[Inject] private List<IUiScreen> _screens;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IUiViel _uiViel;

		readonly ReactiveProperty<Screen> _screen;

		ScreenNavigator()
		{
			_screen = new ReactiveProperty<Screen>();
			Screen = _screen.ToReadOnlyReactiveProperty();
        }

		public void Initialize()
		{
			foreach (var screen in _screens)
			{
				screen.Closed
					.Subscribe(_ => _screen.Value = Ui.Screen.None)
					.AddTo(this);
			}

			_gameCycle.State
				.Subscribe(OnGameCycleChangeHandler)
				.AddTo(this);
		}

		private void OnGameCycleChangeHandler(GameState state)
		{
			CloseActive();

			switch (state)
			{
				case GameState.Lobby:
					Open(Ui.Screen.Lobby);
					break;
				case GameState.WinBattle:
					Open(Ui.Screen.Win);
					break;
			}
		}

		#region IScreenNavigator

		public ReadOnlyReactiveProperty<Screen> Screen { get; }

		public void Open(Screen screen)
		{
			_screen.Value = screen;
			_screens.ForEach(s => s.SetActive(s.Screen == screen));
			_uiViel.SetActive(false);
		}

		public void CloseActive()
		{
			Open(Ui.Screen.None);
		}

		#endregion
	}
}
