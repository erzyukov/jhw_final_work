namespace Game.Analytics
{
	using Game.Profiles;
	using Game.Utilities;
	using System.Collections.Generic;
	using Zenject;
	using UniRx;
	using Game.Core;
	using Game.Configs;
	using UnityEngine;

	public class GameplayAnalytics : ControllerBase, IInitializable
	{
		[Inject] private IAnalyticEventSender _eventSender;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IGameLevel _gameLevel;
		[Inject] private IGameProfileManager _gameProfileManager;
		[Inject] private IScenesManager _scenesManager;
		[Inject] private IGameplayEvents _gameplayEvents;
		[Inject] private IGameHero _gameHero;
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

		private float _levelStartTime;
		private float _waveStartTime;
		private float _mergedAtWaveStart;
		private float _spentTokensAtWaveStart;

		private int WavesCount => _levelsConfig.Levels[_gameProfile.LevelNumber.Value - 1].Waves.Length;

		public void Initialize()
		{
			EventsSubscribes();
			AuxiliarySubscribes();
		}

		private void EventsSubscribes()
		{
			_gameLevel.LevelLoaded
				.Subscribe(OnLevelLoaded)
				.AddTo(this);

			_gameLevel.LevelFinished
				.Subscribe(OnLevelFinished)
				.AddTo(this);

			_gameLevel.WaveStarted
				.Subscribe(OnWaveStarted)
				.AddTo(this);

			_gameLevel.WaveFinished
				.Subscribe(OnWaveFinished)
				.AddTo(this);
		}

		private void AuxiliarySubscribes()
		{
			_gameplayEvents.UnitSummoned
				.Subscribe(v => IncrementProfileProperty(ref _gameProfile.Analytics.SummonTokenSpent, v))
				.AddTo(this);

			_gameplayEvents.UnitsMerged
				.Subscribe(_ => IncrementProfileProperty(ref _gameProfile.Analytics.UnitLevelMergedCount, 1))
				.AddTo(this);
		}

		private void OnLevelLoaded(bool isNewLevel)
		{
			if (isNewLevel)
			{
				_gameProfile.Analytics.SummonTokenSpent = 0;
				_gameProfile.Analytics.UnitLevelMergedCount = 0;
				_gameProfile.Analytics.LevelSpentTime = 0;
			}

			_levelStartTime = Time.time;
			_gameProfile.Analytics.LevelStartsCount++;
			_gameProfile.Analytics.LevelTryCount = 
				(
					_gameProfile.Analytics.LastLevelNumber == _gameProfile.LevelNumber.Value || 
					_gameProfile.Analytics.LastLevelNumber == 0
				)
				? (isNewLevel) ? _gameProfile.Analytics.LevelTryCount + 1 : _gameProfile.Analytics.LevelTryCount
				: 1;
			_gameProfile.Analytics.LastLevelNumber = _gameProfile.LevelNumber.Value;
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
			_eventSender.SendMessage(LevelStartEventKey, properties, true);
		}

		private void OnLevelFinished(GameLevel.Result result)
		{
			int wavesFinished = result == GameLevel.Result.Win 
				? _gameProfile.WaveNumber.Value 
				: _gameProfile.WaveNumber.Value - 1;
			_gameProfile.Analytics.LevelSpentTime += Mathf.RoundToInt(Time.time - _levelStartTime);

			int heroLevel = (_gameProfile.LevelHeroExperience.Value > _gameHero.GetExperienceToLevel)
				? _gameProfile.HeroLevel.Value + 1
				: _gameProfile.HeroLevel.Value;

			var properties = new Dictionary<string, object>
			{
				{ "player_level_number", heroLevel },
				{ "level_number", _gameProfile.LevelNumber.Value },
				{ "level_count", _gameProfile.Analytics.LevelStartsCount },
				{ "wave_amount", WavesCount },
				{ "try_number", _gameProfile.Analytics.LevelTryCount },
				{ "waves_finished", wavesFinished },
				{ "time", _gameProfile.Analytics.LevelSpentTime },
				{ "result", result },
				{ "summon_coins_used", _gameProfile.Analytics.SummonTokenSpent },
				{ "merge_amount", _gameProfile.Analytics.UnitLevelMergedCount },
				{ "xp_amount", _gameProfile.LevelHeroExperience.Value },
				{ "coins_amount", _gameProfile.LevelSoftCurrency.Value },
			};
			_eventSender.SendMessage(LevelFinishEventKey, properties, true);
		}

		private void OnWaveStarted(int waveNumber)
		{
			_waveStartTime = Time.time;
			_mergedAtWaveStart = _gameProfile.Analytics.UnitLevelMergedCount;
			_spentTokensAtWaveStart = _gameProfile.Analytics.SummonTokenSpent;

			var properties = new Dictionary<string, object>
			{
				{ "level_number", _gameProfile.LevelNumber.Value },
				{ "level_count", _gameProfile.Analytics.LevelStartsCount },
				{ "wave_amount", WavesCount },
				{ "try_number", _gameProfile.Analytics.LevelTryCount },
				{ "wave_number", waveNumber },
			};
			_eventSender.SendMessage(WaveStartEventKey, properties, true);
		}

		private void OnWaveFinished(GameLevel.Result result)
		{
			var properties = new Dictionary<string, object>
			{
				{ "player_level_number", _gameProfile.HeroLevel.Value },
				{ "level_number", _gameProfile.LevelNumber.Value },
				{ "level_count", _gameProfile.Analytics.LevelStartsCount },
				{ "wave_amount", WavesCount },
				{ "try_number", _gameProfile.Analytics.LevelTryCount },
				{ "wave_number", _gameProfile.WaveNumber.Value },
				{ "time", Mathf.RoundToInt(Time.time - _waveStartTime) },
				{ "result", result },
				{ "summon_coins_used", _gameProfile.Analytics.SummonTokenSpent - _spentTokensAtWaveStart },
				{ "merge_amount", _gameProfile.Analytics.UnitLevelMergedCount - _mergedAtWaveStart },
			};
			_eventSender.SendMessage(WaveFinishEventKey, properties, true);
		}

		private void IncrementProfileProperty(ref int property, int increment)
		{
			property += increment;
			Save();
		}

		private void Save() => _gameProfileManager.Save();
	}
}
