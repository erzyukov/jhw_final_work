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
		
		ReactiveCommand Rewarded { get; }
		
		bool IsAdAvailable( EAdType type );
		void ShowInterstitialVideo(string place);
		void ShowRevardedVideo(string place);
		void DisplayBanner(string place);
		void HideBanner();
	}
}
