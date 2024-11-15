﻿namespace Game.Ui
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
		[Inject] private IUiVeil _uiViel;

		readonly private ReactiveProperty<Screen> _screen = new ReactiveProperty<Screen>();

		ScreenNavigator()
		{
			Screen = _screen.ToReadOnlyReactiveProperty();
        }

		public void Initialize()
		{
			foreach (var screen in _screens)
			{
				screen.SetActive(false);
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
				case GameState.LoseBattle:
					Open(Ui.Screen.Lose);
					break;
				case GameState.HeroLevelReward:
					Open(Ui.Screen.LevelReward);
					break;
				case GameState.Upgrades:
					Open(Ui.Screen.Upgrades);
					break;
				case GameState.IapShop:
					Open(Ui.Screen.IapShop);
					break;
			}
		}

		#region IScreenNavigator

		public ReadOnlyReactiveProperty<Screen> Screen { get; }

		public void Open(Screen screen)
		{
			if (Screen.Value == screen)
				return;

			_screen.Value = screen;

			_screens.ForEach(s => s.SetActive(s.Screen == screen));
			_uiViel.Fade();
		}

		public void CloseActive()
		{
			if (Screen.Value == Ui.Screen.None)
				return;

			_screen.Value = Ui.Screen.None;
			_screens.ForEach(s => s.SetActive(false));
		}

		#endregion
	}
}