namespace Game.Analytics
{
	using Game.Profiles;
	using Game.Utilities;
	using System.Collections.Generic;
	using Zenject;
	using UniRx;
	using Game.Core;
	using Game.Configs;

	public class GameplayAnalytics : ControllerBase, IInitializable
	{
		[Inject] private IAnalyticEventSender _eventSender;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IGameLevel _gameLevel;
		[Inject] private IGameProfileManager _gameProfileManager;
		[Inject] private IScenesManager _scenesManager;
		[Inject] private GameProfile _gameProfile;
		[Inject] private LevelsConfig _levelsConfig;

		private const string LevelStartEventKey = "level_start";
		private const string LevelFinishEventKey = "level_finish";
		private const string WaveStartEventKey = "wave_start";
		private const string WaveFinishEventKey = "wave_finished";
		private const string BattleStartEventKey = "battle_start";
		private const string BattleFinishEventKey = "battle_finished";
		private const string UnitSummonEventKey = "unit_summon";
		private const string UnitMergeEventKey = "unit_merge";
		private const string UnitUpgradeEventKey = "unit_upgrade";

		private int _lastLevelNumber;

		private int WavesCount => _levelsConfig.Levels[_gameProfile.LevelNumber.Value - 1].Waves.Length;

		public void Initialize()
		{
			_gameLevel.LevelLoaded
				.Subscribe(OnLevelLoaded)
				.AddTo(this);

			_gameProfile.LevelNumber
				.Subscribe(OnLevelNumberChanged)
				.AddTo(this);
		}

		private void OnLevelNumberChanged(int value)
		{
			_lastLevelNumber = value;
			Save();
		}

		private void OnLevelLoaded(bool isNewLevel)
		{
			_gameProfile.Analytics.LevelStartsCount++;
			_gameProfile.Analytics.LevelTryCount = (_lastLevelNumber == _gameProfile.LevelNumber.Value || _lastLevelNumber == 0)
				? (isNewLevel) ? _gameProfile.Analytics.LevelTryCount + 1 : _gameProfile.Analytics.LevelTryCount
				: 1;
			Save();

			var properties = new Dictionary<string, object>
			{
				{ "player_level_number", _gameProfile.HeroLevel.Value },
				{ "level_number", _gameProfile.LevelNumber.Value },
				{ "level_count", _gameProfile.Analytics.LevelStartsCount },
				{ "wave_amount", WavesCount },
				{ "try_number", _gameProfile.Analytics.LevelTryCount },
				{ "continue_level", isNewLevel?0:1 },
			};
			_eventSender.SendMessage(LevelStartEventKey, properties);
		}

		private void Save() => _gameProfileManager.Save();
	}
}
