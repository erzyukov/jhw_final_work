namespace Game.UI
{
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
		[Inject] List<IUiScreen> _screens;

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
					.Subscribe(_ => _screen.Value = UI.Screen.None)
					.AddTo(this);
			}
		}

		#region IScreenNavigator

		public ReadOnlyReactiveProperty<Screen> Screen { get; }

		public void Open(Screen screen)
		{
			_screen.Value = screen;
			_screens.ForEach(s => s.SetActive(s.Screen == screen));
		}

		public void CloseActive()
		{
			Open(UI.Screen.None);
		}

		#endregion
	}
}
