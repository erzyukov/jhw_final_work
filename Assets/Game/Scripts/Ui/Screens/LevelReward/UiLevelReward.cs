namespace Game.Ui
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Core;
	using Game.Profiles;
	using Game.Configs;

	public class UiLevelReward : ControllerBase, IInitializable
	{
		[Inject] private IUiLevelRewardScreen _screen;
		[Inject] private IGameLevel _gameLevel;
		[Inject] private IGameCurrency _gameCurrency;
		[Inject] private GameProfile _profile;
		[Inject] private ExperienceConfig _experienceConfig;

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
			int softReward = _experienceConfig.HeroLevels[_profile.HeroLevel.Value - 1].SoftCurrencyReward;
			_screen.SetSoftRewardAmount(softReward);
			_gameCurrency.AddSoftCurrency(softReward, GameCurrency.Soft.HeroLevelReward, _profile.HeroLevel.Value.ToString());
		}

		private void OnScreenClosedHandler()
		{
			_gameLevel.FinishLevel();
		}
	}
}