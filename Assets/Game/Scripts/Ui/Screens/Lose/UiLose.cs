namespace Game.Ui
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Core;

	public class UiLose : ControllerBase, IInitializable
	{
		[Inject] private IUiLoseScreen _screen;
		[Inject] private IGameHero _hero;
		[Inject] private IGameLevel _gameLevel;

		public void Initialize()
		{
			_screen.Opened
				.Subscribe(_ => OnScreenOpeningHandler())
				.AddTo(this);

			_screen.Closed
				.Subscribe(_ => OnScreenClosedHandler())
				.AddTo(this);
		}

		private void OnScreenOpeningHandler()
		{
			_hero.ConsumeLevelHeroExperience("fail");
		}

		private void OnScreenClosedHandler()
		{
			_gameLevel.FinishLevel();
		}
	}
}