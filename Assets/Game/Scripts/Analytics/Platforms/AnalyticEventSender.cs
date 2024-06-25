namespace Game.Analytics
{
	using System.Collections.Generic;
	using Io.AppMetrica;
	using Newtonsoft.Json;

	public interface IAnalyticEventSender
	{
		void SendEvent( string message, bool immediately = false );
		void SendEvent( string message, Dictionary<string, object> parameters, bool immediately = false );
		void SendAdRevenue( EAdType type, AdRevenueData revenue );
		void SendIapRevenue( IapRevenueData revenue );
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
			string json     = JsonConvert.SerializeObject( parameters );
			AppMetrica.ReportEvent( message, json );

			if (immediately)
				AppMetrica.SendEventsBuffer();
		}

		public void SendAdRevenue( EAdType type, AdRevenueData revenue )
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

		public void SendIapRevenue( IapRevenueData revenue )
		{
			Revenue data	= new( revenue.PriceMicros, revenue.Currency );
			data.Payload	= revenue.Payload;
			data.ProductID	= revenue.ProductID;
			data.Quantity	= revenue.Quantity;

			data.ReceiptValue	= new Revenue.Receipt() {
				Data			= revenue.ReceiptData,
				Signature		= revenue.ReceiptSignature,
				TransactionID	= revenue.ReceiptTransactionID
			};

			AppMetrica.ReportRevenue( data );
		}

#endregion
	}
}