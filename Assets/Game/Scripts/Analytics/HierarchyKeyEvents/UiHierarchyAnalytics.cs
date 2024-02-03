namespace Game.Analytics
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Profiles;
	using Game.Ui;
	using UnityEngine;
	using Screen = Game.Ui.Screen;
	using System.Linq;
	using System.Collections.Generic;

	public class UiHierarchyAnalytics : HierarchyAnalyticsBase, IInitializable
	{
		[Inject] private IScreenNavigator _screenNavigator;
		[Inject] private List<IUiWindow> _windows;
		
		private const string UiEventKey = "UI";
		private const string OpenedPostfixEventKey = "_opened";
		private const string ClosedPostfixEventKey = "_closed";
		private const string WindowOpenDefaultSourceEventKey = "on_button";

		private Screen[] _ignoreScreen = new Screen[] { Screen.None, Screen.Loading, Screen.Lobby };

		private float _previousScreenOpenedTime;
		private float _windowOpenedTime;

		public void Initialize()
		{
			_screenNavigator.Screen
				.StartWith(Screen.None)
				.Pairwise()
				.Subscribe(UiScreenChangedHandler)
				.AddTo(this);

			_windows.ForEach(window =>
			{
				window.Opened
					.Subscribe(_ => OnWindowOpened(window.Type))
					.AddTo(this);

				window.Closed
					.Subscribe(_ => OnWindowClosed(window.Type))
					.AddTo(this);
			});
		}

		private void OnWindowOpened(Window type)
		{
			SendDesignEvent($"{UiEventKey}:{type}{OpenedPostfixEventKey}:{WindowOpenDefaultSourceEventKey}:{LevelKey}:{WaveKey}");
			_windowOpenedTime = Time.time;
		}

		private void OnWindowClosed(Window type)
		{
			SendDesignEvent($"{UiEventKey}:{type}{ClosedPostfixEventKey}:{WindowOpenDefaultSourceEventKey}:{LevelKey}:{WaveKey}", Time.time - _windowOpenedTime);
		}

		void UiScreenChangedHandler(Pair<Screen> pair)
		{
			if (_ignoreScreen.Contains(pair.Previous) && _ignoreScreen.Contains(pair.Current))
				return;

			if (pair.Previous != Screen.None)
			{
				SendDesignEvent($"{UiEventKey}:{pair.Previous}{ClosedPostfixEventKey}:{LevelKey}:{WaveKey}", Time.time - _previousScreenOpenedTime);
			}
			else if (pair.Current != Screen.None)
			{
				SendDesignEvent($"{UiEventKey}:{pair.Current}{OpenedPostfixEventKey}:{LevelKey}:{WaveKey}");
				_previousScreenOpenedTime = Time.time;
			}
		}
	}
}
