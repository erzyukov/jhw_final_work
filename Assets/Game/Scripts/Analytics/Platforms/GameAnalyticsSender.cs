namespace Game.Analytics
{
	using System.Collections.Generic;
	using GameAnalyticsSDK;

	public interface IGameAnalyticsSender
	{
		void SendDesignEvent(string message, Dictionary<string, object> parameters);
		void SendDesignEvent(string message, float value, Dictionary<string, object> parameters);
		void SendResourceEvent(GAResourceFlowType resourceFlowType, string currency, float amount, string itemType, string itemId, Dictionary<string, object> parameters);
		void SendProgressionEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03, IDictionary<string, object> parameters);
	}

	public class GameAnalyticsSender : IGameAnalyticsSender
	{
		#region IAnalyticEventSender

		public void SendDesignEvent(string message, Dictionary<string, object> parameters)
		{
			GameAnalytics.NewDesignEvent(message, parameters);
		}

		public void SendDesignEvent(string message, float value, Dictionary<string, object> parameters)
		{
			GameAnalytics.NewDesignEvent(message, value, parameters);
		}

		public void SendResourceEvent(GAResourceFlowType resourceFlowType, string currency, float amount, string itemType, string itemId, Dictionary<string, object> parameters)
		{
			GameAnalytics.NewResourceEvent(resourceFlowType, currency, amount, itemType, itemId, parameters);
		}

		public void SendProgressionEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03, IDictionary<string, object> parameters)
		{
			GameAnalytics.NewProgressionEvent(progressionStatus, progression01, progression02, progression03, parameters);
		}

		#endregion
	}
}