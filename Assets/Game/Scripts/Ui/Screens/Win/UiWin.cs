namespace Game.Ui
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Core;

	public class UiWin : ControllerBase, IInitializable
	{
		[Inject] private IUiWinScreen _screen;
		[Inject] private IGameHero _hero;
		[Inject] private IGameLevel _gameLevel;

		public void Initialize()
		{
			_screen.Opening
				.Subscribe(_ => OnScreenOpeningHandler())
				.AddTo(this);

			_screen.Closed
				.Subscribe(_ => OnScreenClosedHandler())
				.AddTo(this);
		}

		private void OnScreenOpeningHandler()
		{
			_hero.ConsumeLevelHeroExperience();
		}

		private void OnScreenClosedHandler()
		{
			_gameLevel.FinishLevel();
		}
	}
}