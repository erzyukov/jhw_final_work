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
	using UnityEngine;

	public class UiAnalytics : AnalyticsBase, IInitializable
	{
		[Inject] private IUiWinScreen		_uiWinScreen;
		[Inject] private IUiLoseScreen		_uiLoseScreen;
		[Inject] private IGameHero			_gameHero;
		[Inject] private LevelsConfig		_levelsConfig;

		private const string OpenFinishScreenEventKey = "level_finish_open";
		private const string CloseFinishScreenEventKey = "level_finish_close";

		private int _newLevel;
		private float _showingTime;

		private int WavesCount => _levelsConfig.Levels[GameProfile.LevelNumber.Value - 1].Waves.Length;

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
			_showingTime = Time.time;
			_newLevel = (GameProfile.LevelHeroExperience.Value > _gameHero.GetExperienceToLevel)
				? GameProfile.HeroLevel.Value + 1
				: 0;

			var properties = new Dictionary<string, object>
			{
				{ "wave_amount", WavesCount },
				{ "try_number", GameProfile.Analytics.LevelTryCount },
				{ "is_new_level_reached", _newLevel },
				{ "type", result },
			};
			SendMessage(OpenFinishScreenEventKey, properties);
		}

		private void OnLevelFinishScreenClosed(GameLevel.Result result)
		{
			var properties = new Dictionary<string, object>
			{
				{ "wave_amount", WavesCount },
				{ "try_number", GameProfile.Analytics.LevelTryCount },
				{ "is_new_level_reached", _newLevel },
				{ "time", Mathf.RoundToInt(Time.time - _showingTime) },
				{ "type", result },
			};
			SendMessage(CloseFinishScreenEventKey, properties);
		}
	}
}
