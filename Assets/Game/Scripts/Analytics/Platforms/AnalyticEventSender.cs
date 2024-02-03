namespace Game.Analytics
{
	using System.Collections.Generic;
	using Game.Utilities;
	using UnityEngine;

	public interface IAnalyticEventSender
	{
		void SendEvent(string message, bool immediately = false);
		void SendEvent(string message, Dictionary<string, object> parameters, bool immediately = false);
	}

	public class AnalyticEventSender : IAnalyticEventSender
	{
		#region IAnalyticEventSender

		public void SendEvent(string message, bool immediately = false)
		{
			SendEvent(message, null, immediately);
		}

		public void SendEvent(string message, Dictionary<string, object> parameters, bool immediately = false)
		{
			Debug.LogWarning($">> [Analytics Event] {message}: {parameters.ToString<string, object>()}");
		}

		#endregion
	}
}