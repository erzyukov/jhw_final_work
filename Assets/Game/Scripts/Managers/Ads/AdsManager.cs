namespace Game.Managers
{
	using Sirenix.Utilities;
	using System;
	using System.Collections.Generic;
    using System.Linq;
	using UniRx;
	using UnityEngine;
	using Zenject;
	using Game.Configs;
	using Game.Utilities;
	using Game.Ui;
	using Screen = Game.Ui.Screen;

	public interface IAdsManager
    {
		Dictionary<ERewardedType, ReactiveCommand<Rewarded>> OnCompleted {get;}
		Dictionary<ERewardedType, ReactiveCommand<Rewarded>> OnCanceled {get;}

		void ShowRewardedVideo( ERewardedType place );
		void ShowRewardedVideo( ERewardedType place, Rewarded type );
		void ShowInterstitialVideo();

		bool IsRewardedFree { get; set; }

		BoolReactiveProperty IsPlaying { get; }
		BoolReactiveProperty IsRewardedAvailable { get; }
		BoolReactiveProperty IsInterstitialReady { get; }

		ReadOnlyReactiveProperty<bool> HasInterstitialBlocker { get; }
		ReadOnlyReactiveProperty<bool> HasBannerBlocker { get; }
		void AddRemoveBlocker( EAdsBlocker blocker, bool add );
	}

	public class AdsManager: ControllerBase, IAdsManager, IInitializable
	{
		[Inject] AdsConfig _adsConfig;
		[Inject] IAdsProvider _adsProvider;
		[Inject] IScreenNavigator _screenNavigator;

		private const string DefaultPlace = "Gameplay";

		private float _timeScale;
		private ERewardedType _rewardType;
		private Rewarded _currentRewarded;

		private ReactiveCollection<EAdsBlocker> _blockersInter = new();
		private ReactiveCollection<EAdsBlocker> _blockersBanner = new();

		private string AdPlace =>
			(_screenNavigator.Screen.Value == Screen.None) ? DefaultPlace : _screenNavigator.Screen.Value.ToString();

		public virtual void Initialize()
		{
			AddRemoveBlocker( EAdsBlocker.Mediation_Loading, true );

			AdSubscribe();
			InitIntersitial();
			InitRewarded();

			HasInterstitialBlocker = _blockersInter
				.ObserveCountChanged().Select( count => count > 0 ).ToReadOnlyReactiveProperty();
			HasBannerBlocker = _blockersBanner
				.ObserveCountChanged().Select( count => count > 0 ).ToReadOnlyReactiveProperty();

			_adsProvider.Initialized
				.Subscribe( _ => OnProviderInitialized() )
				.AddTo( this );

			// Disable ads by UI
			_screenNavigator.Screen
				.StartWith( Screen.None )
				.Pairwise()
				.Subscribe( pair => OnUiScreenChanged( pair.Previous, pair.Current ) )
				.AddTo( this );
		}

		void AdSubscribe()
		{
			_adsProvider.AdLoaded
				.Subscribe( OnAdLoaded )
				.AddTo( this );

			_adsProvider.AdOpened
				.Subscribe( OnAdOpened )
				.AddTo( this );

			_adsProvider.AdClosed
				.Subscribe( OnAdClosed )
				.AddTo( this );

			_adsProvider.AdShowFailed
				.Subscribe( OnAdShowFailed )
				.AddTo( this );

			_adsProvider.Rewarded
				.Subscribe( _ => OnRewarded() )
				.AddTo( this );
		}

		void InitIntersitial()
		{
			if (Time.timeScale != 0)
				_timeScale = Time.timeScale;
		}

		void InitRewarded()
		{
#if UNITY_EDITOR
			IsRewardedAvailable.Value = true;
#endif

			((ERewardedType[])Enum.GetValues( typeof( ERewardedType ) ))
				.Where( type => type != ERewardedType.None )
				.ForEach( type =>
				{
					OnCompleted.Add( type, new ReactiveCommand<Rewarded>() );
					OnCanceled.Add( type, new ReactiveCommand<Rewarded>() );
				} );
		}

		void OnProviderInitialized()
		{
			AddRemoveBlocker( EAdsBlocker.Mediation_Loading, false );
			UpdateBannerState();
		}

#region IAdsManager

		public bool IsRewardedFree { get; set; }

		public Dictionary<ERewardedType, ReactiveCommand<Rewarded>> OnCompleted { get; }
			= new Dictionary<ERewardedType, ReactiveCommand<Rewarded>>();
		public Dictionary<ERewardedType, ReactiveCommand<Rewarded>> OnCanceled { get; }
			= new Dictionary<ERewardedType, ReactiveCommand<Rewarded>>();

		public BoolReactiveProperty IsPlaying { get; } = new();
		public BoolReactiveProperty IsRewardedAvailable { get; } = new();
		public BoolReactiveProperty IsInterstitialReady { get; } = new();

		public void ShowRewardedVideo( ERewardedType type )
        {
			_currentRewarded = new Rewarded( type );

			if (IsRewardedFree)
			{
				IsPlaying.Value = true;

				if (OnCompleted.ContainsKey( type ))
					OnCompleted[ type ].Execute( _currentRewarded );

				IsPlaying.Value = false;

				return;
			}

			if (
				Application.isEditor ||
				_adsProvider.IsAdAvailable( EAdType.RewardedVideo ) == false
			)
				return;

			_rewardType = type;
			IsPlaying.Value = true;

			_adsProvider.ShowRevardedVideo( type.ToString() );
		}

		public void ShowRewardedVideo(ERewardedType place, Rewarded type)
        {
			_currentRewarded = type;
			ShowRewardedVideo( place );
		}

		public void ShowInterstitialVideo()
        {
			if (!IsInterstitialReady.Value)
				return;

			if (_adsProvider.IsAdAvailable( EAdType.RewardedVideo ) == false)
				return;
			
			IsInterstitialReady.Value = false;
			IsPlaying.Value = true;

			_adsProvider.ShowInterstitialVideo( AdPlace );
		}

		public ReadOnlyReactiveProperty<bool> HasInterstitialBlocker { get; private set; }
		
		public ReadOnlyReactiveProperty<bool> HasBannerBlocker { get; private set; }

		public void AddRemoveBlocker(EAdsBlocker blocker, bool add)
		{
			if (blocker == EAdsBlocker.None)
				return;

			bool isInter = _adsConfig.BlockersInter.Contains(blocker);
			bool isBanner = _adsConfig.BlockersBanner.Contains(blocker);

			if (add)
			{
				if (isInter) _blockersInter.Add(blocker);
				if (isBanner) _blockersBanner.Add(blocker);
			}
			else
			{
				if (isInter) _blockersInter.Remove(blocker);
				if (isBanner) _blockersBanner.Remove(blocker);
			}

			if (isBanner)
				UpdateBannerState();
		}

#endregion

		void OnUiScreenChanged( Screen previous, Screen current )
		{
			EAdsBlocker GetBlocker(Screen screen) => screen switch
			{
				Screen.Loading => EAdsBlocker.UI_Loading,
				Screen.Win => EAdsBlocker.UI_Info,
				Screen.Lose => EAdsBlocker.UI_Info,
				Screen.LevelReward => EAdsBlocker.UI_Info,
				_ => EAdsBlocker.None,
			};

			EAdsBlocker blockerPre = GetBlocker( previous );
			EAdsBlocker blockerCur = GetBlocker( current );

			AddRemoveBlocker(blockerPre, false);
			AddRemoveBlocker(blockerCur, true);
		}

		void OnAdLoaded(EAdType type)
		{
			switch (type)
			{
				case EAdType.Interstitial:
					IsInterstitialReady.Value = true;
					break;
				case EAdType.RewardedVideo:
					IsRewardedAvailable.Value = true;
					break;
				case EAdType.Banner:
					UpdateBannerState();
					break;
			}
		}

		void OnAdOpened(EAdType type)
		{
			switch (type)
			{
				case EAdType.Interstitial:
				case EAdType.RewardedVideo:
					OnStartPlayAd();
					break;
			}
		}

		void OnAdClosed(EAdType type)
		{
			switch (type)
			{
				case EAdType.Interstitial:
					OnInterstitialAdClosed();
					break;
				case EAdType.RewardedVideo:
					OnRewardVideoClosed();
					break;
			}
		}

		void OnAdShowFailed(EAdType type)
		{
			switch (type)
			{
				case EAdType.Interstitial:
					OnInterstitialAdShowFailed();
					break;
				case EAdType.RewardedVideo:
					OnRewardedVideoAdShowFailed();
					break;
			}
		}

		void OnRewarded()
        {
			if (OnCompleted.ContainsKey( _rewardType ))
				OnCompleted[ _rewardType ].Execute( _currentRewarded );
        }

		void OnStartPlayAd()
		{
			IsPlaying.Value = true;
			SetZeroTimeScale();
		}

		void OnInterstitialAdClosed()
		{
			IsPlaying.Value = false;
			RestoreTimeScale();
		}

		void OnRewardVideoClosed()
		{
			IsPlaying.Value = false;
			RestoreTimeScale();
		}

		void OnInterstitialAdShowFailed()
		{
			IsInterstitialReady.Value = false;
			IsPlaying.Value = false;
		}

		void OnRewardedVideoAdShowFailed()
		{
			IsPlaying.Value = false;
		}

		void SetZeroTimeScale()
		{
			if (Time.timeScale != 0)
				_timeScale = Time.timeScale;

			Time.timeScale		= 0;
		}

		void RestoreTimeScale()
		{
			if (_timeScale != 0)
				Time.timeScale		= _timeScale;
		}

		void UpdateBannerState()
		{
			bool show = !_blockersBanner.Any();

			if (show)
				_adsProvider.DisplayBanner( AdPlace );
			else
				_adsProvider.HideBanner();
		}
	}
}
