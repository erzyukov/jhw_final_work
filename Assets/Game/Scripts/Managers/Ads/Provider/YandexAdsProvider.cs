namespace Game.Managers
{
	using UniRx;
	using Zenject;
	using YG;
	using Game.Utilities;

	public class YandexAdsProvider : ControllerBase, IAdsProvider, IInitializable
	{
		public void Initialize()
		{
			InitializeSubscribe();
			InterstitialSubscribe();
			RewardedVideoSubscribe();
		}

		private void InitializeSubscribe()
		{
			if (YandexGame.SDKEnabled)
				OnInit();
			else
				Observable.FromEvent(
					x => YandexGame.GetDataEvent += x,
					x => YandexGame.GetDataEvent -= x
				)
				.Subscribe( _ => OnInit() )
				.AddTo( this );
		}

		private void OnInit()
		{
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
			Observable.FromEvent(
				x => YandexGame.OpenFullAdEvent += x,
				x => YandexGame.OpenFullAdEvent -= x
			)
				.Subscribe( _ => AdOpened.Execute( EAdType.Interstitial ) )
				.AddTo( this );

			Observable.FromEvent(
				x => YandexGame.ErrorFullAdEvent += x,
				x => YandexGame.ErrorFullAdEvent -= x
			)
				.Subscribe( _ => AdShowFailed.Execute( EAdType.Interstitial ) )
				.AddTo( this );

			Observable.FromEvent(
				x => YandexGame.CloseFullAdEvent += x,
				x => YandexGame.CloseFullAdEvent -= x
			)
				.Subscribe( _ =>
				{
					AdClosed.Execute( EAdType.Interstitial );
					AdLoaded.Execute( EAdType.Interstitial );
				} )
				.AddTo( this );
		}

		private void RewardedVideoSubscribe()
		{
			Observable.FromEvent(
				x => YandexGame.OpenVideoEvent += x,
				x => YandexGame.OpenVideoEvent -= x
			)
				.Subscribe( _ => AdOpened.Execute( EAdType.RewardedVideo ) )
				.AddTo( this );

			Observable.FromEvent(
				x => YandexGame.ErrorVideoEvent += x,
				x => YandexGame.ErrorVideoEvent -= x
			)
				.Subscribe( _ => AdShowFailed.Execute( EAdType.RewardedVideo ) )
				.AddTo( this );

			Observable.FromEvent(
				x => YandexGame.CloseVideoEvent += x,
				x => YandexGame.CloseVideoEvent -= x
			)
				.Subscribe( _ =>
				{
					AdClosed.Execute( EAdType.RewardedVideo );
					AdLoaded.Execute( EAdType.RewardedVideo );
				} )
				.AddTo( this );

			Observable.FromEvent<int>(
					x => YandexGame.RewardVideoEvent += x,
					x => YandexGame.RewardVideoEvent -= x
				)
				.Subscribe( code => Rewarded.Execute( (ERewardedType)code ) )
				.AddTo( this );
		}

#region IAdsProvider

		public ReactiveCommand<EAdType> AdLoaded { get; } = new();

		public ReactiveCommand<EAdType> AdOpened { get; } = new();

		public ReactiveCommand<EAdType> AdClosed { get; } = new();

		public ReactiveCommand<EAdType> AdShowFailed { get; } = new();

		public BoolReactiveProperty IsInitialized { get; } = new();

		public ReactiveCommand<ERewardedType> Rewarded { get; } = new();


		public string InterstitialPlace { get; private set; }
		public string RewardedPlace { get; private set; }
		public string BannerPlace { get; private set; }


		public void DisplayBanner( string place )
		{
			BannerPlace = place;
			YandexGame.StickyAdActivity( true );
			AdOpened.Execute( EAdType.Banner );
		}

		public void HideBanner()
		{
			YandexGame.StickyAdActivity( false );
			AdClosed.Execute( EAdType.Banner );
		}

		public bool IsAdAvailable( EAdType type ) => true;

		public void ShowInterstitialVideo( string place )
		{
			InterstitialPlace = place;
			YandexGame.FullscreenShow();
		}

		public void ShowRevardedVideo( ERewardedType type )
		{
			RewardedPlace = type.ToString();
			YandexGame.RewVideoShow( (int)type );
		}

#endregion

	}
}
