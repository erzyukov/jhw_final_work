namespace Game.Analytics
{
	using System.Collections.Generic;
	using Game.Utilities;
	using UnityEngine;
	using Io.AppMetrica;
	using Newtonsoft.Json;

	public interface IAnalyticEventSender
	{
		void SendEvent( string message, bool immediately = false );
		void SendEvent( string message, Dictionary<string, object> parameters, bool immediately = false );
		void SendAdRevenue( EAdType type, RevenueData revenue );
	}

	public class AnalyticEventSender : IAnalyticEventSender
	{
		#region IAnalyticEventSender

		public void SendEvent( string message, bool immediately = false )
		{
			SendEvent( message, null, immediately );
		}

		public void SendEvent( string message, Dictionary<string, object> parameters, bool immediately = false )
		{
			string json		= JsonConvert.SerializeObject(parameters);
			AppMetrica.ReportEvent( message, json );

			if (immediately)
				AppMetrica.SendEventsBuffer();
		}

		public void SendAdRevenue( EAdType type, RevenueData revenue )
		{
			AdRevenue data			= new(revenue.Revenue, "USD");
			data.AdNetwork			= revenue.NetworkName;
			data.AdPlacementName	= revenue.Placement;
			data.AdUnitId			= revenue.AdUnitIdentifier;
			data.Precision			= revenue.RevenuePrecision;
			data.AdType				= type switch {
				EAdType.RewardedVideo	=> AdType.Rewarded,
				EAdType.Interstitial	=> AdType.Interstitial,
				EAdType.Banner			=> AdType.Banner,
				_						=> AdType.Other,
			};

			AppMetrica.ReportAdRevenue( data );
		}

		#endregion
	}
}