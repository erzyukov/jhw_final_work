namespace Game.Analytics
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Managers;
	using System.Collections.Generic;
	using UnityEngine;

	public class MediationAnalytics : AnalyticsBase, IInitializable
	{
		[Inject] protected IAdsProvider _adsProvider;

		private const string LoadedEvent			= "video_ads_available";
		private const string OpenedEvent			= "video_ads_started";
		private const string ClosedEvent			= "video_ads_watch";
		private const string RevenueCustomEvent		= "ad_succeed";

		public void Initialize()
		{
			_adsProvider.AdLoaded
				.Where( t => t != EAdType.Banner )
				.Subscribe( type => SendMediationEvent(type, LoadedEvent) )
				.AddTo( this );

			_adsProvider.AdOpened
				.Where( t => t != EAdType.Banner )
				.Subscribe( type => SendMediationEvent(type, OpenedEvent) )
				.AddTo( this );

			_adsProvider.AdClosed
				.Subscribe( type => SendMediationEvent(type, ClosedEvent) )
				.AddTo( this );

			_adsProvider.AdRevenued
				.Subscribe( data => SendMediationRevenue( data.Item1, data.Item2 ) )
				.AddTo( this );

			_adsProvider.AdOpened
				.Where( t => t != EAdType.Banner )
				.Subscribe( type =>
				{
					switch (type)
					{
						case EAdType.Interstitial:
							GameProfile.Analytics.InterWatchNumber++;
							break;
						case EAdType.RewardedVideo:
							GameProfile.Analytics.RewardedWatchNumber++;
							break;
					}
				} )
				.AddTo( this );
		}

		private void SendMediationEvent( EAdType type, string key )
		{
			string place = type switch {
				EAdType.Interstitial		=> _adsProvider.InterstitialPlace,
				EAdType.RewardedVideo		=> _adsProvider.RewardedPlace,
				EAdType.Banner				=> _adsProvider.BannerPlace,
				_ => "unknown",
			};

			var properties = new Dictionary<string, object>
			{
				{ "type",	type.ToString() },
				{ "place",	place },
				{ "time",	Time.time },
			};
			SendMessage( key, properties );
		}

		private void SendMediationRevenue( EAdType type, RevenueData data )
		{
			var properties = new Dictionary<string, object>
			{
				{ "ad_format",	type.ToString() },
				{ "revenue",	data.Revenue },
				{ "currency",	"USD" },
				{ "time",		Time.time },
			};

			SendMessage( RevenueCustomEvent, properties );
			SendAdRevenue( type, data );
		}
	}
}
