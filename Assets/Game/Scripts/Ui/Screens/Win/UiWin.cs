namespace Game.Ui
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Core;
	using DG.Tweening;
	using UnityEngine;
	using Game.Profiles;
	using Game.Managers;
	using System;

	public class UiWin : ControllerBase, IInitializable
	{
		[Inject] private IUiWinScreen _screen;
		[Inject] private IGameHero _hero;
		[Inject] private IGameLevel _gameLevel;
		[Inject] private IGameCurrency _gameCurrency;
		[Inject] private GameProfile _gameProfile;
		[Inject] private IAdsManager _adsManager;

		private const float MultiplyAnimationDuration = 1f;
		private const float RewardAnimationDuration = 1.5f;
		private const float RewardAnimationDelay = 0.7f;
		private Tween _sliderAnimation;
		private Tween _bonusAnimation;
		private int _rewardBonusAmount;

		public void Initialize()
		{
			_screen.Opened
				.Subscribe( _ => OnScreenOpeningHandler() )
				.AddTo( this );

			_screen.Closed
				.Subscribe( _ => OnScreenClosedHandler() )
				.AddTo( this );

			_screen.RewardedClicked
				.Subscribe( _ => OnRewardedClicked() )
				.AddTo( this );

			_adsManager.OnCompleted[ERewardedType.WinReward]
				.Subscribe( _ => OnRewardedVideoComplete() )
				.AddTo( this );
		}

		public override void Dispose()
		{
			_sliderAnimation.Kill();
			_bonusAnimation.Kill();
			base.Dispose();
		}

		private void OnScreenOpeningHandler()
		{
			_screen.SetRequestParentActive( true );
			_screen.SetSkipButtonInteractable( true );
			
			_screen.SetCompleteParentActive( false );
			_screen.SetCloseButtonInteractable( false );

			_hero.ConsumeLevelHeroExperience( "win" );

			_screen.SetLevelRewardValue( _gameProfile.LevelSoftCurrency.Value );

			_sliderAnimation = DOVirtual.Float( 0, 1, MultiplyAnimationDuration, ( p ) =>
			{
				float multiplier = 1 - Mathf.Abs( p - 0.5f ) * 2;
				_rewardBonusAmount = Mathf.RoundToInt( _gameProfile.LevelSoftCurrency.Value * multiplier );
				_screen.SetSelectLightAlpha( multiplier );
				_screen.SetSliderValue( p );
			} )
				.SetLoops( -1, LoopType.Yoyo )
				.SetEase( Ease.InOutSine );
		}

		private void OnScreenClosedHandler()
		{
			_gameLevel.FinishLevel();
		}

		private void OnRewardedClicked()
		{
			_screen.SetSkipButtonInteractable( false );
			_sliderAnimation.Pause();
		}

		private void OnRewardedVideoComplete()
		{
			_screen.SetAdRewardValue( _rewardBonusAmount );

			_screen.SetRequestParentActive( false );
			_screen.SetCloseButtonInteractable( true );

			_screen.SetAdRewardActive( true );
			_screen.SetCompleteParentActive( true );

			int reward = _gameProfile.LevelSoftCurrency.Value;
			int rewardWithBonus = reward + _rewardBonusAmount;
			
			_gameCurrency.AddLevelSoftCurrency( _rewardBonusAmount );

			Observable.Timer( TimeSpan.FromSeconds( RewardAnimationDelay ) )
				.Subscribe( _ => StartAnimation( reward, rewardWithBonus, () => _screen.SetAdRewardActive( false ) ) )
				.AddTo( this );
		}

		private void StartAnimation( int reward, int rewardWithBonus, Action callback )
		{
			float newReward = 0;
			float adReward = 0;

			_bonusAnimation = DOVirtual.Float( 0, 1, RewardAnimationDuration, ( p ) =>
			{
				newReward = Mathf.Lerp( reward, rewardWithBonus, p );
				adReward = Mathf.Lerp( _rewardBonusAmount, 0, p );
				
				_screen.SetLevelRewardValue( Mathf.RoundToInt( newReward ) );
				_screen.SetAdRewardValue( Mathf.RoundToInt( adReward ) );
			} )
				.SetEase( Ease.OutSine )
				.OnComplete( () => callback.Invoke() );
		}
	}
}