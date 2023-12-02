namespace Game.Ui
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Core;
	using Game.Profiles;

	public class UiLose : ControllerBase, IInitializable
	{
		[Inject] private IUiLoseScreen _screen;
		[Inject] private IGameHero _hero;
		[Inject] private IGameLevel _level;
		[Inject] private GameProfile _profile;

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
			_screen.SetHeroLevel(_profile.HeroLevel.Value);
			_screen.SetExperienceRatio(_hero.GetExperienceRatio());
			_screen.SetLevelReward(0);
		}

		private void OnScreenClosedHandler()
		{
			_level.FinishLevel(false);
		}
	}
}