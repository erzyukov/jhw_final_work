namespace Game.Managers
{
	using Zenject;
	using UniRx;
	using Game.Utilities;
	using Game.Configs;
	using Logger = Game.Logger;

	public class ApplovinAdsProvider : ControllerBase, IAdsProvider, IInitializable
	{
		[Inject] AdsConfig		_adsConfig;
		[Inject] DevConfig		_devConfig;

		private const string	DefaultPlace = "Gameplay";

		public void Initialize()
		{
			InitializeSubscribe();
			InterstitialSubscribe();
			RewardedVideoSubscribe();
		}

		public override void Dispose()
		{
			MaxSdkCallbacks.OnSdkInitializedEvent -= OnSdkInitialized;
		}

		private void InitializeSubscribe()
		{
			MaxSdkCallbacks.OnSdkInitializedEvent += OnSdkInitialized;

			if (_devConfig.Build == DevConfig.BuildType.Debug)
			{
				MaxSdk.SetVerboseLogging( true );
				string gaid = "bbdf33dd-9e91-4807-8e7e-6d50010a574d";
				MaxSdk.SetTestDeviceAdvertisingIdentifiers(new string[]{gaid, gaid});
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

			Observable.NextFrame()
				.Subscribe( _ =>
				{
					AdLoaded.Execute( EAdType.Banner );
					AdLoaded.Execute( EAdType.RewardedVideo );
					AdLoaded.Execute( EAdType.Interstitial );
				} )
				.AddTo( this );
		}

		private void InterstitialSubscribe()
		{
			//Observable.FromEvent(
			//	x => YandexGame.OpenFullAdEvent += x,
			//	x => YandexGame.OpenFullAdEvent -= x
			//)
			//	.Subscribe( _ => AdOpened.Execute( EAdType.Interstitial ) )
			//	.AddTo( this );

			//Observable.FromEvent(
			//	x => YandexGame.ErrorFullAdEvent += x,
			//	x => YandexGame.ErrorFullAdEvent -= x
			//)
			//	.Subscribe( _ => AdShowFailed.Execute( EAdType.Interstitial ) )
			//	.AddTo( this );

			//Observable.FromEvent(
			//	x => YandexGame.CloseFullAdEvent += x,
			//	x => YandexGame.CloseFullAdEvent -= x
			//)
			//	.Subscribe( _ =>
			//	{
			//		AdClosed.Execute( EAdType.Interstitial );
			//		AdLoaded.Execute( EAdType.Interstitial );
			//	} )
			//	.AddTo( this );
		}

		private void RewardedVideoSubscribe()
		{
			//Observable.FromEvent(
			//	x => YandexGame.OpenVideoEvent += x,
			//	x => YandexGame.OpenVideoEvent -= x
			//)
			//	.Subscribe( _ => AdOpened.Execute( EAdType.RewardedVideo ) )
			//	.AddTo( this );

			//Observable.FromEvent(
			//	x => YandexGame.ErrorVideoEvent += x,
			//	x => YandexGame.ErrorVideoEvent -= x
			//)
			//	.Subscribe( _ => AdShowFailed.Execute( EAdType.RewardedVideo ) )
			//	.AddTo( this );

			//Observable.FromEvent(
			//	x => YandexGame.CloseVideoEvent += x,
			//	x => YandexGame.CloseVideoEvent -= x
			//)
			//	.Subscribe( _ =>
			//	{
			//		AdClosed.Execute( EAdType.RewardedVideo );
			//		AdLoaded.Execute( EAdType.RewardedVideo );
			//	} )
			//	.AddTo( this );

			//Observable.FromEvent<int>(
			//		x => YandexGame.RewardVideoEvent += x,
			//		x => YandexGame.RewardVideoEvent -= x
			//	)
			//	.Subscribe( code => Rewarded.Execute( (ERewardedType)code ) )
			//	.AddTo( this );
		}


#region IAdsProvider

		public ReactiveCommand<EAdType> AdLoaded		{ get; } = new();

		public ReactiveCommand<EAdType> AdOpened		{ get; } = new();

		public ReactiveCommand<EAdType> AdClosed		{ get; } = new();

		public ReactiveCommand<EAdType> AdShowFailed	{ get; } = new();

		public BoolReactiveProperty IsInitialized		{ get; } = new();

		public ReactiveCommand<ERewardedType> Rewarded	{ get; } = new();


		public string InterstitialPlace { get; private set; }	= DefaultPlace;
		public string RewardedPlace { get; private set; }		= DefaultPlace;
		public string BannerPlace { get; private set; }			= DefaultPlace;


		public void DisplayBanner( string place )
		{
			BannerPlace = place;
			//YandexGame.StickyAdActivity( true );
			AdOpened.Execute( EAdType.Banner );
		}

		public void HideBanner()
		{
			//YandexGame.StickyAdActivity( false );
			AdClosed.Execute( EAdType.Banner );
		}

		public bool IsAdAvailable( EAdType type ) => true;

		public void ShowInterstitialVideo( string place )
		{
			InterstitialPlace = place;
			//YandexGame.FullscreenShow();
		}

		public void ShowRevardedVideo( ERewardedType type )
		{
			RewardedPlace = type.ToString();
			//YandexGame.RewVideoShow( (int)type );
		}

#endregion

	}
}
