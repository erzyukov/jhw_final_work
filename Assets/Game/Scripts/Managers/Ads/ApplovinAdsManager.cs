namespace Game.Managers
{
	using System.Collections.Generic;
	using UniRx;
	using Game.Ui;
	using Game.Utilities;

	public class ApplovinAdsManager : AdsManager
	{
		private readonly List<Screen>	BeforAdScreen = 
			new() {Screen.Win, Screen.Lose, Screen.LevelReward};

		private float		_interInterval;
		private ITimer		_interTimer			= new Timer(true);

		public override void Initialize()
		{
			base.Initialize();

			_interInterval		= AdsConfig.InterstitialInterval;
			SetInterTimer();

			IsPlaying
				.Where( v => !v )
				.Subscribe( _ => SetInterTimer() )
				.AddTo( this );

			Observable.CombineLatest(
				IsPlaying,
				HasInterstitialBlocker,
				( p, b ) => p || b
			)
				.Subscribe( needPause => SetTimerPaused( needPause ) )
				.AddTo( this );

			// Disable ads by UI
			ScreenNavigator.Screen
				.Where( s => s != Screen.None )
				.Pairwise()
				.Subscribe( pair => OnUiScreenChanged( pair.Previous, pair.Current ) )
				.AddTo( this );

		}

		private void SetTimerPaused( bool isPaused )
		{
			if (isPaused)
				_interTimer.Pause();
			else
				_interTimer.Unpause();
		}

		private void SetInterTimer() =>
			_interTimer.Set( _interInterval );

		protected void OnUiScreenChanged( Screen previous, Screen current )
		{
			if (
				_interTimer.IsReady == false ||
				BeforAdScreen.Contains( previous ) == false ||
				GameProfile.LevelNumber.Value < AdsConfig.InterActiveLevelNumber ||
				current != Screen.Lobby
			)
				return;

			ShowInterstitialVideo();
		}
	}
}
