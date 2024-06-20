namespace Game.Managers
{
	using Game.Analytics;
	using UniRx;

	public interface IAdsProvider
	{
		BoolReactiveProperty IsInitialized { get; }

		ReactiveCommand<EAdType> AdLoaded { get; }
		ReactiveCommand<EAdType> AdOpened { get; }
		ReactiveCommand<EAdType> AdClosed { get; }
		ReactiveCommand<EAdType> AdShowFailed { get; }

		ReactiveCommand<ERewardedType> Rewarded { get; }
		
		ReactiveCommand<(EAdType, RevenueData)> AdRevenued { get; }

		string InterstitialPlace { get; }
		string RewardedPlace { get; }
		string BannerPlace { get; }

		bool IsAdAvailable( EAdType type );
		void ShowInterstitialVideo( string place );
		void ShowRevardedVideo( ERewardedType type );
		void DisplayBanner( string place );
		void HideBanner();
	}
}
