namespace Game.Managers
{
	using System;
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
				ObserveEvent( YandexGame.GetDataEvent, () => OnInit() );
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
			ObserveEvent( YandexGame.OpenFullAdEvent, () => AdOpened.Execute( EAdType.Interstitial ) );
			ObserveEvent( YandexGame.ErrorFullAdEvent, () => AdShowFailed.Execute( EAdType.Interstitial ) );
			ObserveEvent( YandexGame.CloseFullAdEvent, () =>
			{
				AdClosed.Execute( EAdType.Interstitial );
				AdLoaded.Execute( EAdType.Interstitial );
			} );
		}

		private void RewardedVideoSubscribe()
		{
			ObserveEvent( YandexGame.OpenVideoEvent, () => AdOpened.Execute( EAdType.RewardedVideo ) );
			ObserveEvent( YandexGame.ErrorVideoEvent, () => AdShowFailed.Execute( EAdType.RewardedVideo ) );
			ObserveEvent( YandexGame.CloseVideoEvent, () =>
			{
				AdClosed.Execute( EAdType.RewardedVideo );
				AdLoaded.Execute( EAdType.RewardedVideo );
			} );

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

		public void DisplayBanner( string place ) =>
			YandexGame.StickyAdActivity( true );

		public void HideBanner() =>
			YandexGame.StickyAdActivity( false );

		public bool IsAdAvailable( EAdType type ) => true;

		public void ShowInterstitialVideo( string place ) =>
			YandexGame.FullscreenShow();

		public void ShowRevardedVideo( ERewardedType type ) =>
			YandexGame.RewVideoShow( (int)type );

#endregion

		private void ObserveEvent(Action action, Action callback)
		{
			Observable.FromEvent(
					x => action += x,
					x => action -= x
				)
				.Subscribe( _ => callback.Invoke() )
				.AddTo( this );
		}

	}
}
