namespace Game.Analytics
{
	using Game.Core;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using System.Collections.Generic;
	using Game.Profiles;
	using Game.Ui;
	using Game.Configs;

	public class UiAnalytics : ControllerBase, IInitializable
	{
		[Inject] private IAnalyticEventSender _eventSender;
		[Inject] private IUiWinScreen _uiWinScreen;
		[Inject] private IUiLoseScreen _uiLoseScreen;
		[Inject] private IGameLevel _gameLevel;
		[Inject] private IGameHero _gameHero;
		[Inject] private GameProfile _gameProfile;
		[Inject] private LevelsConfig _levelsConfig;

		private const string OpenFinishScreenEventKey = "level_finish_open";
		private const string CloseFinishScreenEventKey = "level_finish_close";

		private int _newLevel;
		
		private int WavesCount => _levelsConfig.Levels[_gameProfile.LevelNumber.Value - 1].Waves.Length;

		public void Initialize()
		{
			_uiWinScreen.Opening
				.Subscribe(_ => OnLevelFinishScreenOpened(GameLevel.Result.Win))
				.AddTo(this);

			_uiLoseScreen.Opening
				.Subscribe(_ => OnLevelFinishScreenOpened(GameLevel.Result.Fail))
				.AddTo(this);

			_uiWinScreen.Closed
				.Subscribe(_ => OnLevelFinishScreenClosed(GameLevel.Result.Win))
				.AddTo(this);

			_uiLoseScreen.Closed
				.Subscribe(_ => OnLevelFinishScreenClosed(GameLevel.Result.Fail))
				.AddTo(this);
		}

		private void OnLevelFinishScreenOpened(GameLevel.Result result)
		{
			_newLevel = (_gameProfile.LevelHeroExperience.Value > _gameHero.GetExperienceToLevel)
				? _gameProfile.HeroLevel.Value + 1
				: 0;

			var properties = new Dictionary<string, object>
			{
				{ "player_level_number", _gameProfile.HeroLevel.Value },
				{ "level_number", _gameProfile.LevelNumber.Value },
				{ "wave_amount", WavesCount },
				{ "try_number", _gameProfile.Analytics.LevelTryCount },
				{ "is_new_level_reached", _newLevel },
				{ "type", result },
			};
			_eventSender.SendMessage(OpenFinishScreenEventKey, properties);
		}

		private void OnLevelFinishScreenClosed(GameLevel.Result result)
		{
			var properties = new Dictionary<string, object>
			{
				{ "player_level_number", _gameProfile.HeroLevel.Value },
				{ "level_number", _gameProfile.LevelNumber.Value },
				{ "wave_amount", WavesCount },
				{ "try_number", _gameProfile.Analytics.LevelTryCount },
				{ "is_new_level_reached", _newLevel },
				{ "type", result },
			};
			_eventSender.SendMessage(CloseFinishScreenEventKey, properties);
		}
	}
}
