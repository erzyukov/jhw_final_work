namespace Game.Analytics
{
	using Game.Utilities;
	using Zenject;
	using System.Collections.Generic;
	using Game.Profiles;
	using Game.Configs;
	using GameAnalyticsSDK;
	using UnityEngine;

	abstract public class HierarchyAnalyticsBase : ControllerBase
	{
		[Inject] private LevelsConfig _levelsConfig;
		[Inject] protected IGameProfileManager GameProfileManager;
		[Inject] private IGameAnalyticsSender _eventSender;

		protected const string BattleDefaultTypeEventKey = "normal";

		private const string RegionEventKey = "Region_";
		private const string LevelEventKey = "Level_";
		private const string WaveEventKey = "Wave_";

		protected GameProfile GameProfile => GameProfileManager.GameProfile;

		protected int RegionNumber => (int)_levelsConfig.Levels[ GameProfile.LevelNumber.Value - 1 ].Region + 1;
		protected string LevelKey => $"{RegionEventKey}{RegionNumber}-{LevelEventKey}{GameProfile.LevelNumber.Value}";
		protected string WaveKey => $"{WaveEventKey}{(GameProfile.WaveNumber.Value == 0 ? 1 : GameProfile.WaveNumber.Value)}";

		private Dictionary<string, object> GlobalProperties => new Dictionary<string, object>
			{
				{ "player_level_number", GameProfile.HeroLevel.Value },
				{ "level_number", GameProfile.LevelNumber.Value },
				{ "level_count", GameProfile.Analytics?.LevelStartsCount },
				{ "soft_balance", GameProfile.SoftCurrency?.Value },
				{ "energy_balance", GameProfile.Energy.Amount },
			};

		protected void SendDesignEvent( string key ) =>
			_eventSender.SendDesignEvent( key, GlobalProperties );

		protected void SendDesignEvent( string key, float value ) =>
			_eventSender.SendDesignEvent( key, value, GlobalProperties );

		protected void SendResourceEvent( GAResourceFlowType resourceFlowType, string currency, float amount, string itemType, string itemId ) =>
			_eventSender.SendResourceEvent( resourceFlowType, currency, amount, itemType, itemId, GlobalProperties );

		protected void SendProgressionEvent( GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03 ) =>
			_eventSender.SendProgressionEvent( progressionStatus, progression01, progression02, progression03, GlobalProperties );
	}
}