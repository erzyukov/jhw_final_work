namespace Game.Analytics
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using System.Collections.Generic;
	using Game.Profiles;
	using System.Linq;

	abstract public class AnalyticsBase : ControllerBase
	{
		[Inject] protected IGameProfileManager GameProfileManager;

		[Inject] private IAnalyticEventSender _eventSender;

		protected GameProfile GameProfile => GameProfileManager.GameProfile;

		protected void SendMessage(string key, Dictionary<string, object> properties, bool immediately = false)
		{
			var globalProperties = new Dictionary<string, object>
			{
				{ "player_level_number", GameProfile.HeroLevel.Value },
				{ "level_number", GameProfile.LevelNumber.Value },
				{ "level_count", GameProfile.Analytics.LevelStartsCount },
				{ "soft_balance", GameProfile.SoftCurrency.Value },
				{ "energy_balance", GameProfile.Energy.Amount },
			};

			properties.ToList().ForEach(x => globalProperties.Add(x.Key, x.Value));

			_eventSender.SendEvent(key, globalProperties, immediately);
		}

		protected void SendAdRevenue( EAdType type, AdRevenueData revenue )
		{
			_eventSender.SendAdRevenue( type, revenue );
		}

		protected void SendIapRevenue( IapRevenueData revenue )
		{
			_eventSender.SendIapRevenue( revenue );
		}
    }
}