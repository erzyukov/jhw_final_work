namespace Game.Ui
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Core;
	using Game.Managers;
	using Game.Profiles;

	public class UiLose : ControllerBase, IInitializable
	{
		[Inject] private IUiLoseScreen _screen;
		[Inject] private IGameHero _hero;
		[Inject] private IGameLevel _gameLevel;
		[Inject] private IGameCurrency _gameCurrency;
		[Inject] private IAdsManager _adsManager;
		[Inject] private GameProfile _gameProfile;

		int _bonusTokenAmount;

		public void Initialize()
		{
			_screen.Opened
				.Subscribe(_ => OnScreenOpeningHandler())
				.AddTo(this);

			_screen.Closed
				.Subscribe(_ => OnScreenClosedHandler())
				.AddTo(this);

			_adsManager.OnCompleted[ERewardedType.LoseRevive]
				.Subscribe( _ => OnRewardedVideoComplete() )
				.AddTo( this );
		}

		private void OnScreenOpeningHandler()
		{
			_bonusTokenAmount = _gameLevel.GetCurrentWaveSummonCurrency();
			_screen.SetBonusTokenAmount( _bonusTokenAmount );

			SetRewardedStateActive( _gameProfile.ReviveAttemptsCount > 0 );

			_gameProfile.ReviveAttemptsCount--;
		}

		private void OnScreenClosedHandler()
		{
			_hero.ConsumeLevelHeroExperience("fail");

			_gameLevel.FinishLevel();
		}

		private void OnRewardedVideoComplete()
		{
			_gameCurrency.AddSummonCurrency(_bonusTokenAmount);
			_gameLevel.GoToLevel( _gameProfile.LevelNumber.Value, _gameProfile.WaveNumber.Value );
		}

		private void SetRewardedStateActive( bool value )
		{
			_screen.SetRewardedBlockActive( value );
			_screen.SetDefaulBlockActive( !value );
			_screen.SetCloseButtonInteractable( !value );
		}
	}
}