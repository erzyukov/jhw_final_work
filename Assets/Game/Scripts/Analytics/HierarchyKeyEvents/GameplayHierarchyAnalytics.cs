namespace Game.Analytics
{
	using Game.Profiles;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Core;
	using Game.Configs;
	using UnityEngine;
	using Game.Units;

	public class GameplayHierarchyAnalytics : HierarchyAnalyticsBase, IInitializable
	{
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IGameplayEvents _gameplayEvents;
		[Inject] private IGameUpgrades _gameUpgrades;
		[Inject] private UpgradesConfig _upgradesConfig;
		[Inject] private GameProfile _gameProfile;

		private const string UnitUpgradeEventKey = "UnitUpgrade";
		private const string HeroLevelUpEventKey = "PlayerLevelUp";
		private const string HeroLevelNumberEventKey = "Level_";

		private const string BattleEventKey = "Battle";
		private const string BattleStartEventKey = "Start";
		private const string BattleWinEventKey = "Win";
		private const string BattleFailEventKey = "Fail";

		private const string SummonEventKey = "Summon";
		private const string MergeEventKey = "Merge";

		private const string TacticalEventKey = "Tactical";
		private const string TacticalStartEventKey = "Start";
		private const string TacticalFinishEventKey = "Finish";

		private float _battleStartTime;
		private float _tacticalStartTime;

		public void Initialize()
		{
			EventsSubscribes();
		}

		private void EventsSubscribes()
		{
			_gameUpgrades.Upgraded
				.Subscribe(OnUnitUpgraded)
				.AddTo(this);

			_gameProfile.HeroLevel
				.Skip(1)
				.Subscribe(OnHeroLevelUp)
				.AddTo(this);

			_gameplayEvents.BattleStarted
				.Subscribe(_ => OnBattleStarted())
				.AddTo(this);

			_gameplayEvents.BattleWon
				.Subscribe(_ => OnBattleWon())
				.AddTo(this);

			_gameplayEvents.BattleLost
				.Subscribe(_ => OnBattleFailed())
				.AddTo(this);

			_gameCycle.State
				.Where(v => v == GameState.TacticalStage)
				.Subscribe(_ => OnTacticalStarted())
				.AddTo(this);

			_gameCycle.State
				.Where(v => v == GameState.BattleStage)
				.Subscribe(_ => OnTacticalFinished())
				.AddTo(this);

			_gameplayEvents.UnitSummoned
				.Subscribe(OnUnitSummoned)
				.AddTo(this);

			_gameplayEvents.UnitsMerged
				.Subscribe(OnUnitsMerged)
				.AddTo(this);
		}

		private void OnUnitUpgraded(Species species)
		{
			int upgradeLevel			= _gameUpgrades.GetUnitLevel(species);
			UnitUpgradesConfig upgrade	= _upgradesConfig.UnitsUpgrades[species];
			int maxPriceLevel			= Mathf.Clamp(upgradeLevel, 1, upgrade.Price.Length + 1);
			int prevPriceLevel			= Mathf.Max(maxPriceLevel - 2, 0);
			int previousUpgradePrice	= upgrade.Price[prevPriceLevel];

			SendDesignEvent($"{UnitUpgradeEventKey}:{(int)species}:{upgradeLevel}", previousUpgradePrice);
		}

		private void OnHeroLevelUp(int levelNumber)
		{
			SendDesignEvent($"{HeroLevelUpEventKey}:{HeroLevelNumberEventKey}{levelNumber}:{LevelKey}:{WaveKey}");
		}

		private void OnTacticalStarted()
		{
			_tacticalStartTime = Time.time;
			SendDesignEvent($"{TacticalEventKey}:{BattleDefaultTypeEventKey}:{TacticalStartEventKey}:{LevelKey}:{WaveKey}");
		}

		private void OnTacticalFinished()
		{
			SendDesignEvent($"{TacticalEventKey}:{BattleDefaultTypeEventKey}:{TacticalFinishEventKey}:{LevelKey}:{WaveKey}", Time.time - _tacticalStartTime);
		}

		private void OnBattleStarted()
		{
			_battleStartTime = Time.time;
			SendDesignEvent($"{BattleEventKey}:{BattleDefaultTypeEventKey}:{BattleStartEventKey}:{LevelKey}:{WaveKey}");
		}

		private void OnBattleWon()
		{
			SendDesignEvent($"{BattleEventKey}:{BattleDefaultTypeEventKey}:{BattleWinEventKey}:{LevelKey}:{WaveKey}", Time.time - _battleStartTime);
		}

		private void OnBattleFailed()
		{
			SendDesignEvent($"{BattleEventKey}:{BattleDefaultTypeEventKey}:{BattleFailEventKey}:{LevelKey}:{WaveKey}", Time.time - _battleStartTime);
		}

		private void OnUnitSummoned(IUnitFacade unit)
		{
			SendDesignEvent($"{SummonEventKey}:{BattleDefaultTypeEventKey}:{(int)unit.Species}:{LevelKey}:{WaveKey}", _gameProfile.HeroField.Units.Count);
		}

		private void OnUnitsMerged(IUnitFacade unit)
		{
			SendDesignEvent($"{MergeEventKey}:{BattleDefaultTypeEventKey}:{(int)unit.Species}:{LevelKey}:{WaveKey}", _gameProfile.HeroField.Units.Count);
		}
	}
}
