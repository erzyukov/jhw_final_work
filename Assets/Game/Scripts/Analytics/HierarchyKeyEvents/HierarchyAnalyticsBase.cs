namespace Game.Analytics
{
	using Game.Utilities;
	using Zenject;
	using System.Collections.Generic;
	using Game.Profiles;

	abstract public class HierarchyAnalyticsBase : ControllerBase
	{
		[Inject] protected IGameProfileManager GameProfileManager;

		[Inject] private IGameAnalyticsSender _eventSender;

		protected GameProfile GameProfile => GameProfileManager.GameProfile;

		private Dictionary<string, object> GlobalProperties => new Dictionary<string, object>
			{
				{ "player_level_number", GameProfile.HeroLevel.Value },
				{ "level_number", GameProfile.LevelNumber.Value },
				{ "level_count", GameProfile.Analytics.LevelStartsCount },
				{ "soft_balance", GameProfile.SoftCurrency.Value },
				{ "energy_balance", GameProfile.Energy.Amount },
			};

		protected void SendDesignEvent(string key) =>
			_eventSender.SendDesignEvent(key, GlobalProperties);

		protected void SendDesignEvent(string key, float value) =>
			_eventSender.SendDesignEvent(key, value, GlobalProperties);
	}
}