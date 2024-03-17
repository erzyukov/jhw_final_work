namespace Game.Managers
{
	using UniRx;

	public interface IAdsProvider
	{
		ReactiveCommand Initialized { get; }

		ReactiveCommand<EAdType> AdLoaded { get; }
		ReactiveCommand<EAdType> AdOpened { get; }
		ReactiveCommand<EAdType> AdClosed { get; }
		ReactiveCommand<EAdType> AdShowFailed { get; }

		ReactiveCommand<ERewardedType> Rewarded { get; }

		bool IsAdAvailable( EAdType type );
		void ShowInterstitialVideo( string place );
		void ShowRevardedVideo( ERewardedType type );
		void DisplayBanner( string place );
		void HideBanner();
	}
}
