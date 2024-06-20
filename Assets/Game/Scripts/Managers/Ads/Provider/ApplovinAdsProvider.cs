namespace Game.Managers
{
	using System;
	using UniRx;
	using Zenject;
	using Game.Utilities;
	using Game.Configs;
	using Logger = Game.Logger;
	using static MaxSdkBase;
	using Game.Analytics;

	public class ApplovinAdsProvider : ControllerBase, IAdsProvider, IInitializable
	{
		[Inject] AdsConfig		_adsConfig;
		[Inject] DevConfig		_devConfig;

		private const string	DefaultPlace = "Gameplay";

		private string InterUnitId		=> _adsConfig.MaxSdkInterstitialUnitId;
		private string RewardUnitId		=> _adsConfig.MaxSdkRewardedUnitId;
		private string BannerUnitId		=> _adsConfig.MaxSdkBannerUnitId;

		private int			_interstitialLoadRetryAttempt;
		private int			_rewardedLoadRetryAttempt;

		public void Initialize()
		{
			InitializeSubscribe();
			InterstitialSubscribe();
			RewardedVideoSubscribe();
			BannerSubscribe();
		}

		public override void Dispose()
		{
			MaxSdkCallbacks.OnSdkInitializedEvent				-= OnSdkInitialized;

			MaxSdkCallbacks.Interstitial.OnAdLoadedEvent		-= OnInterstitialLoaded;
			MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent	-= OnInterstitialLoadFailed;
			MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent -= OnInterstitialDisplayFailed;
			MaxSdkCallbacks.Interstitial.OnAdHiddenEvent		-= OnInterstitialHidden;
			MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent		-= OnInterstitialDisplayed;
			MaxSdkCallbacks.Interstitial.OnAdClickedEvent		-= OnInterstitialClicked;

			MaxSdkCallbacks.Rewarded.OnAdLoadedEvent			-= OnRewardedLoaded;
			MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent		-= OnRewardedLoadFailed;
			MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent			-= OnRewardedDisplayed;
			MaxSdkCallbacks.Rewarded.OnAdClickedEvent			-= OnRewardedClicked;
			MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent		-= OnRewardedRevenuePaid;
			MaxSdkCallbacks.Rewarded.OnAdHiddenEvent			-= OnRewardedHidden;
			MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent		-= OnRewardedDisplayFailed;
			MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent	-= OnRewardedRewardReceived;

			MaxSdkCallbacks.Banner.OnAdLoadedEvent				-= OnBannerAdLoaded;
			MaxSdkCallbacks.Banner.OnAdLoadFailedEvent			-= OnBannerAdLoadFailed;
			MaxSdkCallbacks.Banner.OnAdClickedEvent				-= OnBannerAdClicked;
			MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent			-= OnBannerAdRevenuePaid;
			MaxSdkCallbacks.Banner.OnAdExpandedEvent			-= OnBannerAdExpanded;
			MaxSdkCallbacks.Banner.OnAdCollapsedEvent			-= OnBannerAdCollapsed;
		}

		private void InitializeSubscribe()
		{
			MaxSdkCallbacks.OnSdkInitializedEvent				+= OnSdkInitialized;

			if (_devConfig.Build == DevConfig.BuildType.Debug)
			{
				MaxSdk.SetVerboseLogging( true );
				string gaid = "bbdf33dd-9e91-4807-8e7e-6d50010a574d";
				//MaxSdk.SetTestDeviceAdvertisingIdentifiers(new string[]{gaid, gaid});
			}

			MaxSdk.SetSdkKey( _adsConfig.MaxSdkKey );
			MaxSdk.InitializeSdk();
		}

		private void OnSdkInitialized( MaxSdkBase.SdkConfiguration sdkConfiguration )
		{
			if (_devConfig.Build == DevConfig.BuildType.Debug)
			{
				Logger.Log( Logger.Module.Ads, $"IsSuccessfullyInitialized: {sdkConfiguration.IsSuccessfullyInitialized} | IsTestModeEnabled: {sdkConfiguration.IsTestModeEnabled}" );

#if !UNITY_EDITOR
				MaxSdk.ShowMediationDebugger();
#endif
			}

			IsInitialized.Value = true;
		}

#region Interstitial

		private void InterstitialSubscribe()
		{
			MaxSdkCallbacks.Interstitial.OnAdLoadedEvent		+= OnInterstitialLoaded;
			MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent	+= OnInterstitialLoadFailed;
			MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialDisplayFailed;
			MaxSdkCallbacks.Interstitial.OnAdHiddenEvent		+= OnInterstitialHidden;
			MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent		+= OnInterstitialDisplayed;
			MaxSdkCallbacks.Interstitial.OnAdClickedEvent		+= OnInterstitialClicked;
			MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent	+= OnInterstitialRevenuePaid;

			LoadInterstitial();
		}

		private void LoadInterstitial()
		{
			MaxSdk.LoadInterstitial( InterUnitId );
		}

		private void OnInterstitialLoaded( string adUnitId, MaxSdkBase.AdInfo adInfo )
		{
			Logger.Log( Logger.Module.Ads, $"Interstitial loaded! {adInfo.NetworkName}" );

			_interstitialLoadRetryAttempt = 0;

			AdLoaded.Execute( EAdType.Interstitial );
		}

		private void OnInterstitialLoadFailed( string adUnitId, MaxSdkBase.ErrorInfo errorInfo )
		{
			Logger.Log( Logger.Module.Ads, $"Interstitial Load Failed! {errorInfo.Code}: {errorInfo.Message}" );

			_interstitialLoadRetryAttempt++;
			double retryDelay = Math.Pow(2, Math.Min(6, _interstitialLoadRetryAttempt));

			Observable.Timer( TimeSpan.FromSeconds( retryDelay ) )
				.Subscribe( _ => LoadInterstitial() )
				.AddTo( this );
		}

		private void OnInterstitialDisplayFailed( string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo )
		{
			AdShowFailed.Execute( EAdType.Interstitial );

			LoadInterstitial();
		}

		private void OnInterstitialHidden( string adUnitId, MaxSdkBase.AdInfo adInfo )
		{
			AdClosed.Execute( EAdType.Interstitial );

			LoadInterstitial();
		}

		private void OnInterstitialDisplayed( string adUnitId, MaxSdkBase.AdInfo adInfo )
		{
			AdOpened.Execute( EAdType.Interstitial );
		}

		private void OnInterstitialClicked( string arg1, MaxSdkBase.AdInfo info ) {}

		private void OnInterstitialRevenuePaid( string adUnitId, AdInfo adInfo )
		{
			Logger.Log( Logger.Module.Ads, $"Interstitial Revenue: {adInfo.Revenue}: {adInfo.RevenuePrecision}" );

			RevenueData data	= ToRevenueData( adInfo );
			AdRevenued.Execute( (EAdType.Interstitial, data) );
		}

#endregion

#region Rewarded

		private void RewardedVideoSubscribe()
		{
			MaxSdkCallbacks.Rewarded.OnAdLoadedEvent			+= OnRewardedLoaded;
			MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent		+= OnRewardedLoadFailed;
			MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent			+= OnRewardedDisplayed;
			MaxSdkCallbacks.Rewarded.OnAdClickedEvent			+= OnRewardedClicked;
			MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent		+= OnRewardedRevenuePaid;
			MaxSdkCallbacks.Rewarded.OnAdHiddenEvent			+= OnRewardedHidden;
			MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent		+= OnRewardedDisplayFailed;
			MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent	+= OnRewardedRewardReceived;

			LoadRewardedAd();
		}

		private void LoadRewardedAd()
		{
			MaxSdk.LoadRewardedAd( RewardUnitId );
		}

		private void OnRewardedLoaded( string adUnitId, MaxSdkBase.AdInfo adInfo )
		{
			Logger.Log( Logger.Module.Ads, $"Rewarded loaded! {adInfo.NetworkName}" );

			_rewardedLoadRetryAttempt = 0;

			AdLoaded.Execute( EAdType.RewardedVideo );
		}

		private void OnRewardedLoadFailed( string adUnitId, MaxSdkBase.ErrorInfo errorInfo )
		{
			Logger.Log( Logger.Module.Ads, $"Rewarded Load Failed! {errorInfo.Code}: {errorInfo.Message}" );

			_rewardedLoadRetryAttempt++;
			double retryDelay = Math.Pow(2, Math.Min(6, _rewardedLoadRetryAttempt));

			Observable.Timer( TimeSpan.FromSeconds( _rewardedLoadRetryAttempt ) )
				.Subscribe( _ => LoadRewardedAd() )
				.AddTo( this );
		}

		private void OnRewardedDisplayed( string adUnitId, AdInfo adInfo )
		{
			AdOpened.Execute( EAdType.RewardedVideo );
		}

		private void OnRewardedClicked( string adUnitId, AdInfo adInfo ) {}

		private void OnRewardedRevenuePaid( string adUnitId, AdInfo adInfo )
		{
			Logger.Log( Logger.Module.Ads, $"Rewarded Revenue: {adInfo.Revenue}: {adInfo.RevenuePrecision}" );

			RevenueData data	= ToRevenueData( adInfo );
			AdRevenued.Execute( (EAdType.RewardedVideo, data) );
		}

		private void OnRewardedHidden( string arg1, AdInfo info )
		{
			AdClosed.Execute( EAdType.RewardedVideo );
			
			LoadRewardedAd();
		}

		private void OnRewardedDisplayFailed( string adUnitId, ErrorInfo errorInfo, AdInfo adInfo )
		{
			AdShowFailed.Execute( EAdType.RewardedVideo );

			LoadRewardedAd();
		}

		private void OnRewardedRewardReceived( string adUnitId, Reward reward, AdInfo adInfo )
		{
			Rewarded.Execute( ERewardedType.None );
		}

		#endregion

#region Banner

		private void BannerSubscribe()
		{
			BannerPlace = MaxSdkBase.BannerPosition.BottomCenter.ToString();
			MaxSdk.CreateBanner( BannerUnitId, MaxSdkBase.BannerPosition.BottomCenter );
			MaxSdk.SetBannerBackgroundColor( BannerUnitId, new( 0, 0, 0, 0 ) );

			MaxSdkCallbacks.Banner.OnAdLoadedEvent      += OnBannerAdLoaded;
			MaxSdkCallbacks.Banner.OnAdLoadFailedEvent  += OnBannerAdLoadFailed;
			MaxSdkCallbacks.Banner.OnAdClickedEvent     += OnBannerAdClicked;
			MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaid;
			MaxSdkCallbacks.Banner.OnAdExpandedEvent    += OnBannerAdExpanded;
			MaxSdkCallbacks.Banner.OnAdCollapsedEvent   += OnBannerAdCollapsed;
		}

		private void OnBannerAdLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			AdLoaded.Execute( EAdType.Banner );
		}

		private void OnBannerAdLoadFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
		{
			AdShowFailed.Execute( EAdType.Banner );
		}

		private void OnBannerAdClicked(string adUnitId, MaxSdkBase.AdInfo adInfo) {}

		private void OnBannerAdRevenuePaid(string adUnitId, MaxSdkBase.AdInfo adInfo)
		{
			Logger.Log( Logger.Module.Ads, $"Banner Revenue: {adInfo.Revenue}: {adInfo.RevenuePrecision}" );

			RevenueData data	= ToRevenueData( adInfo );
			AdRevenued.Execute( (EAdType.Banner, data) );
		}

		private void OnBannerAdExpanded(string adUnitId, MaxSdkBase.AdInfo adInfo)  {}

		private void OnBannerAdCollapsed(string adUnitId, MaxSdkBase.AdInfo adInfo) {}

#endregion
#region IAdsProvider

		public ReactiveCommand<EAdType> AdLoaded		{ get; } = new();

		public ReactiveCommand<EAdType> AdOpened		{ get; } = new();

		public ReactiveCommand<EAdType> AdClosed		{ get; } = new();

		public ReactiveCommand<EAdType> AdShowFailed	{ get; } = new();

		public BoolReactiveProperty IsInitialized		{ get; } = new();

		public ReactiveCommand<ERewardedType> Rewarded	{ get; } = new();

		public ReactiveCommand<(EAdType, RevenueData)> AdRevenued { get; } = new();

		public string InterstitialPlace { get; private set; }	= DefaultPlace;
		public string RewardedPlace { get; private set; }		= DefaultPlace;
		public string BannerPlace { get; private set; }			= DefaultPlace;


		public void DisplayBanner( string place )
		{
			MaxSdk.ShowBanner( BannerUnitId );
			AdOpened.Execute( EAdType.Banner );
		}

		public void HideBanner()
		{
			MaxSdk.HideBanner( BannerUnitId );
			AdClosed.Execute( EAdType.Banner );
		}

		public bool IsAdAvailable( EAdType type ) =>
			type switch {
				EAdType.Interstitial	=> MaxSdk.IsInterstitialReady( InterUnitId ),
				EAdType.RewardedVideo	=> MaxSdk.IsRewardedAdReady( RewardUnitId ),
				_ => false,
			};

		public void ShowInterstitialVideo( string place )
		{
			InterstitialPlace = place;
			MaxSdk.ShowInterstitial( InterUnitId );
		}

		public void ShowRevardedVideo( ERewardedType type )
		{
			RewardedPlace = type.ToString();
			MaxSdk.ShowRewardedAd( RewardUnitId, RewardedPlace );
		}

#endregion

		private RevenueData ToRevenueData( AdInfo adInfo )
		{
			return new(){
				AdUnitIdentifier	= adInfo.AdUnitIdentifier,
				AdFormat			= adInfo.AdFormat,
				NetworkName			= adInfo.NetworkName,
				NetworkPlacement	= adInfo.NetworkPlacement,
				Placement			= adInfo.Placement,
				Revenue				= adInfo.Revenue,
				RevenuePrecision	= adInfo.RevenuePrecision
			};
		}
	}
}
