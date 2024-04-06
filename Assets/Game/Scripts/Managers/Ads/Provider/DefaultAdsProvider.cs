namespace Game.Managers
{
	using Game.Utilities;
	using UniRx;
	using Zenject;

	public class DefaultAdsProvider : ControllerBase, IAdsProvider, IInitializable
	{

		private const string DefaultPlace = "Gameplay";

		public void Initialize()
		{
			IsInitialized.Value = true;
		}

#region IAdsProvider

		public ReactiveCommand<EAdType> AdLoaded { get; } = new();

		public ReactiveCommand<EAdType> AdOpened { get; } = new();

		public ReactiveCommand<EAdType> AdClosed { get; } = new();

		public ReactiveCommand<EAdType> AdShowFailed { get; } = new();

		public BoolReactiveProperty IsInitialized { get; } = new();

		public ReactiveCommand<ERewardedType> Rewarded { get; } = new();


		public string InterstitialPlace { get; private set; } = DefaultPlace;
		public string RewardedPlace { get; private set; } = DefaultPlace;
		public string BannerPlace { get; private set; } = DefaultPlace;


		public void DisplayBanner( string place )
		{
			BannerPlace = place;
			AdOpened.Execute( EAdType.Banner );
		}

		public void HideBanner()
		{
			AdClosed.Execute( EAdType.Banner );
		}

		public bool IsAdAvailable( EAdType type ) => true;

		public void ShowInterstitialVideo( string place )
		{
			InterstitialPlace = place;
			AdOpened.Execute( EAdType.Interstitial );
			AdClosed.Execute( EAdType.Interstitial );
			AdLoaded.Execute( EAdType.Interstitial );
		}

		public void ShowRevardedVideo( ERewardedType type )
		{
			RewardedPlace = type.ToString();
			AdOpened.Execute( EAdType.Interstitial );
			AdClosed.Execute( EAdType.Interstitial );
			Rewarded.Execute( type );
		}

#endregion
	}
}
