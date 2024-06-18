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
	using Game.Gameplay;
	using Game.Units;
	using System.Linq;

	public class GameplayAnalytics : AnalyticsBase, IInitializable
	{
		[Inject] private IGameLevel				_gameLevel;
		[Inject] private IGameProfileManager	_gameProfileManager;
		[Inject] private IGameplayEvents		_gameplayEvents;
		[Inject] private IGameUpgrades			_gameUpgrades;
		[Inject] private LevelsConfig			_levelsConfig;
		[Inject] private CurrencyConfig			_currencyConfig;
		[Inject] private UpgradesConfig			_upgradesConfig;

		private const string LevelStartEventKey		= "level_start";
		private const string LevelFinishEventKey	= "level_finish";
		private const string WaveStartEventKey		= "wave_start";
		private const string WaveFinishEventKey		= "wave_finished";
		private const string BattleStartEventKey	= "battle_start";
		private const string BattleFinishEventKey	= "battle_finished";
		private const string UnitSummonEventKey		= "unit_summon";
		private const string UnitMergeEventKey		= "unit_merge";
		private const string UnitUpgradeEventKey	= "unit_upgrade";

		private float _levelStartTime;
		private float _waveStartTime;
		private float _mergedAtWaveStart;
		private float _spentTokensAtWaveStart;
		private float _battleStartTime;

		private int WavesCount => _levelsConfig.Levels[GameProfile.LevelNumber.Value - 1].Waves.Length;

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

			_gameplayEvents.BattleStarted
				.Subscribe(OnBattleStarted)
				.AddTo(this);

			_gameplayEvents.BattleWon
				.Subscribe(data => OnBattleFinished(data, GameLevel.Result.Win))
				.AddTo(this);

			_gameplayEvents.BattleLost
				.Subscribe(data => OnBattleFinished(data, GameLevel.Result.Fail))
				.AddTo(this);

			_gameplayEvents.UnitSummoned
				.Subscribe(OnUnitSummoned)
				.AddTo(this);

			_gameplayEvents.UnitsMerged
				.Subscribe(OnUnitsMerged)
				.AddTo(this);

			_gameUpgrades.Upgraded
				.Subscribe(OnUnitUpgraded)
				.AddTo(this);
		}

		private void AuxiliarySubscribes()
		{
			_gameplayEvents.UnitSummoned
				.Subscribe(_ => IncrementProfileProperty(ref GameProfile.Analytics.SummonTokenSpent, _currencyConfig.UnitSummonPrice))
				.AddTo(this);

			_gameplayEvents.UnitsMerged
				.Subscribe(_ => IncrementProfileProperty(ref GameProfile.Analytics.UnitLevelMergedCount, 1))
				.AddTo(this);
		}

		private void OnLevelLoaded(bool isNewLevel)
		{
			if (isNewLevel)
			{
				GameProfile.Analytics.SummonTokenSpent = 0;
				GameProfile.Analytics.UnitLevelMergedCount = 0;
				GameProfile.Analytics.LevelSpentTime = 0;
			}

			_levelStartTime = Time.time;
			GameProfile.Analytics.LevelStartsCount++;
			GameProfile.Analytics.LevelTryCount = 
				(
					GameProfile.Analytics.LastLevelNumber == GameProfile.LevelNumber.Value || 
					GameProfile.Analytics.LastLevelNumber == 0
				)
				? (isNewLevel) ? GameProfile.Analytics.LevelTryCount + 1 : GameProfile.Analytics.LevelTryCount
				: 1;
			GameProfile.Analytics.LastLevelNumber = GameProfile.LevelNumber.Value;
			Save();

			var properties = new Dictionary<string, object>
			{
				{ "wave_amount", WavesCount },
				{ "try_number", GameProfile.Analytics.LevelTryCount },
				{ "continue_level", isNewLevel?0:1 },
			};
			SendMessage(LevelStartEventKey, properties, true);
		}

		private void OnLevelFinished(GameLevel.Result result)
		{
			int wavesFinished = result == GameLevel.Result.Win 
				? GameProfile.WaveNumber.Value 
				: GameProfile.WaveNumber.Value - 1;
			GameProfile.Analytics.LevelSpentTime += Mathf.RoundToInt(Time.time - _levelStartTime);

			var properties = new Dictionary<string, object>
			{
				{ "wave_amount", WavesCount },
				{ "try_number", GameProfile.Analytics.LevelTryCount },
				{ "waves_finished", wavesFinished },
				{ "time", GameProfile.Analytics.LevelSpentTime },
				{ "result", result },
				{ "summon_coins_used", GameProfile.Analytics.SummonTokenSpent },
				{ "merge_amount", GameProfile.Analytics.UnitLevelMergedCount },
				{ "xp_amount", GameProfile.LevelHeroExperience.Value },
				{ "coins_amount", GameProfile.LevelSoftCurrency.Value },
			};
			SendMessage(LevelFinishEventKey, properties, true);
		}

		private void OnWaveStarted(int waveNumber)
		{
			_waveStartTime = Time.time;
			_mergedAtWaveStart = GameProfile.Analytics.UnitLevelMergedCount;
			_spentTokensAtWaveStart = GameProfile.Analytics.SummonTokenSpent;

			var properties = new Dictionary<string, object>
			{
				{ "wave_amount", WavesCount },
				{ "try_number", GameProfile.Analytics.LevelTryCount },
				{ "wave_number", waveNumber },
			};
			SendMessage(WaveStartEventKey, properties, true);
		}

		private void OnWaveFinished(GameLevel.Result result)
		{
			var properties = new Dictionary<string, object>
			{
				{ "wave_amount", WavesCount },
				{ "try_number", GameProfile.Analytics.LevelTryCount },
				{ "wave_number", GameProfile.WaveNumber.Value },
				{ "time", Mathf.RoundToInt(Time.time - _waveStartTime) },
				{ "result", result },
				{ "summon_coins_used", GameProfile.Analytics.SummonTokenSpent - _spentTokensAtWaveStart },
				{ "merge_amount", GameProfile.Analytics.UnitLevelMergedCount - _mergedAtWaveStart },
			};
			SendMessage(WaveFinishEventKey, properties, true);
		}

		private void OnBattleStarted(BattlefieldData data)
		{
			_battleStartTime = Time.time;

			var properties = new Dictionary<string, object>
			{
				{ "wave_amount", WavesCount },
				{ "try_number", GameProfile.Analytics.LevelTryCount },
				{ "wave_number", GameProfile.WaveNumber.Value },
				{ "time", Mathf.RoundToInt(Time.time - _waveStartTime) },
				{ "enemy_numbers", data.EnemyField.Units.Count },
				{ "unit_numbers", data.HeroField.AliveUnitsCount },
				{ "enemy_json", "" },
				{ "unit_json", "" },
			};
			SendMessage(BattleStartEventKey, properties, true);
		}

		private void OnBattleFinished(BattlefieldData data, GameLevel.Result result)
		{
			var properties = new Dictionary<string, object>
			{
				{ "wave_amount", WavesCount },
				{ "try_number", GameProfile.Analytics.LevelTryCount },
				{ "wave_number", GameProfile.WaveNumber.Value },
				{ "time", Mathf.RoundToInt(Time.time - _waveStartTime) },
				{ "enemy_numbers", data.EnemyField.Units.Count },
				{ "unit_numbers", data.HeroField.AliveUnitsCount },
				{ "enemy_json", "" },
				{ "unit_json", "" },
				{ "battle_time", Mathf.RoundToInt(Time.time - _battleStartTime) },
				{ "result", result },
			};
			SendMessage(BattleFinishEventKey, properties, true);
		}

		private void OnUnitSummoned(IUnitFacade unit)
		{
			int summonPrice = _currencyConfig.UnitSummonPrice;
			int upgradeLevel = _gameUpgrades.GetUnitLevel(unit.Species);

			var properties = new Dictionary<string, object>
			{
				{ "unit_level_number", upgradeLevel },
				{ "unit_id", (int)unit.Species },
				{ "unit_merge_level", unit.GradeIndex + 1 },
				{ "power", unit.Power },
				{ "summon_coins_used", summonPrice },
			};
			SendMessage(UnitSummonEventKey, properties);
		}

		private void OnUnitsMerged(IUnitFacade unit)
		{
			int upgradeLevel = _gameUpgrades.GetUnitLevel(unit.Species);

			var properties = new Dictionary<string, object>
			{
				{ "unit_level_number", upgradeLevel },
				{ "unit_id", (int)unit.Species },
				{ "unit_merge_level", unit.GradeIndex + 1 },
				{ "power", unit.Power },
			};
			SendMessage(UnitMergeEventKey, properties);
		}

		private void OnUnitUpgraded(Species species)
		{
			int upgradeLevel = _gameUpgrades.GetUnitLevel(species);
			int maxLevelUnlocked = GameProfile.Levels.Where(l => l.Unlocked.Value).Select((l, i) => i).LastOrDefault() + 1;
			UnitUpgradesConfig upgrade = _upgradesConfig.UnitsUpgrades[species];
			int maxPriceLevel = Mathf.Clamp(upgradeLevel, 1, upgrade.Price.Length + 1);
			int previousUpgradePrice = upgrade.Price[maxPriceLevel - 2];

			var properties = new Dictionary<string, object>
			{
				{ "unit_level_number", upgradeLevel },
				{ "unit_id", (int)species },
				{ "coins_used", previousUpgradePrice },
			};
			SendMessage(UnitUpgradeEventKey, properties);
		}

		private void IncrementProfileProperty(ref int property, int increment)
		{
			property += increment;
			Save();
		}

		private void Save() => _gameProfileManager.Save();
	}
}
