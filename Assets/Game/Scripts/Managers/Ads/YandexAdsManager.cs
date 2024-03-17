namespace Game.Managers
{
	using Zenject;
	using UniRx;
	using Game.Configs;
	using Game.Utilities;

	public class YandexAdsManager : AdsManager, IInitializable
	{
		[Inject] AdsConfig _adsConfig;
		[Inject] IAdsManager _controller;

		private float _interInterval;
		private ITimer _interTimer = new Timer(true);

		public override void Initialize()
		{
			base.Initialize();

			_interInterval = _adsConfig.InterstitialInterval;
			SetInterTimer();

			_controller.IsInterstitialReady
				.Where( isReady => isReady && _interTimer.Remained < _adsConfig.SecondsToShowAdsTimer )
				.Subscribe( _ => _interTimer.Set( _adsConfig.SecondsToShowAdsTimer ) )
				.AddTo( this );

			_controller.IsPlaying
				.Where( v => !v )
				.Subscribe( _ => SetInterTimer() )
				.AddTo( this );

			Observable.CombineLatest(
				_controller.IsPlaying,
				_controller.HasInterstitialBlocker,
				( p, b ) => p || b
			)
				.Subscribe( needPause => SetTimerPaused( needPause ) )
				.AddTo( this );
		}

		private void SetTimerPaused( bool isPaused )
		{
			if (isPaused)
				_interTimer.Pause();
			else
				_interTimer.Unpause();
		}

		void SetInterTimer() =>
			_interTimer.Set( _interInterval );
	}
}
