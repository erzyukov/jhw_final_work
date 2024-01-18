namespace Game.Analytics
{
	using System.Collections.Generic;
	using Game.Utilities;
	using UnityEngine;

	public interface IAnalyticEventSender
	{
		void SendMessage(string message, bool immediately = false);
		void SendMessage(string message, Dictionary<string, object> parameters, bool immediately = false);
	}

	public class AnalyticEventSender : IAnalyticEventSender
	{
		#region IAnalyticEventSender

		public void SendMessage(string message, bool immediately = false)
		{
			SendMessage(message, null, immediately);
		}

		public void SendMessage(string message, Dictionary<string, object> parameters, bool immediately = false)
		{
			Debug.LogWarning($">> [Analytics Event] {message}: {parameters.ToString<string, object>()}");
		}

		#endregion
	}
}