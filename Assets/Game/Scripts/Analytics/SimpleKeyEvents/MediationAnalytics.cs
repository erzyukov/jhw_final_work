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

		private const string LoadedEvent = "video_ads_available";
		private const string OpenedEvent = "video_ads_started";
		private const string ClosedEvent = "video_ads_watch";

		public void Initialize()
		{
			_adsProvider.AdLoaded
				.Subscribe( type => SendMediationEvent(type, LoadedEvent) )
				.AddTo( this );

			_adsProvider.AdOpened
				.Subscribe( type => SendMediationEvent(type, OpenedEvent) )
				.AddTo( this );

			_adsProvider.AdClosed
				.Subscribe( type => SendMediationEvent(type, ClosedEvent) )
				.AddTo( this );
		}

		private void SendMediationEvent( EAdType type, string key )
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

			string place = type switch {
				EAdType.Interstitial		=> _adsProvider.InterstitialPlace,
				EAdType.RewardedVideo		=> _adsProvider.RewardedPlace,
				EAdType.Banner				=> _adsProvider.BannerPlace,
				_ => "unknown",
			};

			var properties = new Dictionary<string, object>
			{
				{ "type",	type },
				{ "place",	place },
				{ "time",	Time.time },
			};
			SendMessage( key, properties );
		}
	}
}
