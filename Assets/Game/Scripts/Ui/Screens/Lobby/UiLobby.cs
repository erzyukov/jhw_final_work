namespace Game.Ui
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Core;
	using Game.Profiles;
	using Game.Configs;
	using UnityEngine;
	using DG.Tweening;
	using System;

	public class UiLobby : ControllerBase, IInitializable
	{
		[Inject] private IUiLobbyScreen _lobbyScreen;
		[Inject] private IUiMessage _uiMessage;
		[Inject] private IGameLevel _gameLevel;
		[Inject] private IGameEnergy _gameEnergy;
		[Inject] private IGameCurrency _gameCurrency;
		[Inject] private GameProfile _profile;
		[Inject] private LevelsConfig _levelsConfig;
		[Inject] private EnergyConfig _energyConfig;
		[Inject] private ILocalizator _localizator;
		[Inject] private IContinueLevelRequest _continueLevelRequest;
		[Inject] private IResourceEvents _resourceEvents;

		private const float SwitchDuration = 0.8f;
		private const string LevelTitlePrefixKey = "level";
		private const string lastWavePrefixKey = "uiLastWave";
		private const string playKey = "play";
		private const string pricePrefixKey = "pricePrefix";

		private string _levelTitlePrefix;

		private bool CanPlayEnergyFree => WaveNumber != 0 || _gameLevel.MaxOpened < _energyConfig.FreeLevelTo;

		private int _selectedLevelIndex;

		public void Initialize()
		{
			_levelTitlePrefix = _localizator.GetString( LevelTitlePrefixKey );
			_selectedLevelIndex = _profile.LevelNumber.Value - 1;

			_lobbyScreen.Opened
				.Subscribe( _ => OnScreenOpeningHandler() )
				.AddTo( this );

			_lobbyScreen.PlayButtonClicked
				.Subscribe( _ => OnPlayButtonClickedHandler() )
				.AddTo( this );

			_lobbyScreen.PreviousLevelClicked
				.Subscribe( _ => OnPreviousLevelClicked() )
				.AddTo( this );

			_lobbyScreen.NextLevelClicked
				.Subscribe( _ => OnNextLevelClicked() )
				.AddTo( this );
		}

		private void OnScreenOpeningHandler()
		{
			_selectedLevelIndex = _profile.LevelNumber.Value - 1;

			SetLevelInfo( _selectedLevelIndex );

			UpdateScreenState();

			_lobbyScreen.SetPlayPriceText( _energyConfig.LevelPrice.ToString() );
		}

		private void SetLevelInfo( int levelIndex )
		{
			LevelConfig levelConfig = _levelsConfig.Levels[levelIndex];

			_lobbyScreen.SetTitle( $"{_levelTitlePrefix} {levelConfig.Title}" );

			_lobbyScreen.SetLastWaveActive( WaveNumber != 0 );
			string waveInfo = $"{_localizator.GetString(lastWavePrefixKey)} {WaveNumber}/{levelConfig.Waves.Length}";
			_lobbyScreen.SetLastWaveValue( waveInfo );

			_lobbyScreen.SetPlayPriceActive( CanPlayEnergyFree == false );

			_lobbyScreen.SetLevelIcon( levelConfig.Icon );
		}

		private int WaveNumber =>
			( _selectedLevelIndex == _profile.LevelNumber.Value - 1 )
				? _profile.WaveNumber.Value
				: 0;

		private void OnPlayButtonClickedHandler()
		{
			if (_gameLevel.MaxOpened < _energyConfig.FreeLevelTo)
				GoToLevelFree();
			else if (WaveNumber == 0)
				GoToLevel();
			else
				_continueLevelRequest.ShowRequest( () => GoToLevel( true ), () => GoToLevelFree() );
		}

		private void GoToLevel( bool resetWave = false )
		{
			int targetWave = (resetWave)? 0: WaveNumber;

			if (_gameEnergy.TryPayLevel())
			{
				if (resetWave)
					_gameCurrency.ResetLevelSoftCurrency();

				_gameLevel.GoToLevel( _selectedLevelIndex + 1, targetWave );
			}
			else
			{
				_uiMessage.ShowMessage( UiMessage.NotEnoughEnergy );

				_resourceEvents.LowEnergyAlert.Execute();
			}
		}

		private void GoToLevelFree() =>
			_gameLevel.GoToLevel( _selectedLevelIndex + 1, WaveNumber );

		private void OnPreviousLevelClicked() =>
			AnimateSwitching( 0.5f, 0, () => SetSelectedPrew() );

		private void OnNextLevelClicked() =>
			AnimateSwitching( 0.5f, 1, () => SetSelectedNext() );

		private void SetSelectedPrew() =>
			_selectedLevelIndex = Mathf.Max( 0, _selectedLevelIndex - 1 );

		private void SetSelectedNext() =>
			_selectedLevelIndex = Mathf.Min( _levelsConfig.Levels.Length - 1, _selectedLevelIndex + 1 );

		private void AnimateSwitching(float from, float to, Action callback)
		{
			SetSwitcherActive( false );
			_lobbyScreen.SetPlayButtonEnabled( false );

			DOVirtual.Float( from, to, SwitchDuration, ( t ) =>
			{
				_lobbyScreen.SetIconNormalizedPosition( t );
			} ).SetEase( Ease.InOutCubic )
			.OnComplete(() =>
			{
				callback.Invoke();
				SetLevelInfo( _selectedLevelIndex );
				UpdateScreenState();
				ResetSwitcherPosition();
			} );
		}

		private void UpdateScreenState()
		{
			int prevIndex = Mathf.Max( 0 , _selectedLevelIndex - 1);
			int nextIndex = Mathf.Min( _levelsConfig.Levels.Length - 1, _selectedLevelIndex + 1 );

			_lobbyScreen.SetPrewLevelIcon( _levelsConfig.Levels[prevIndex].Icon );
			_lobbyScreen.SetNextLevelIcon( _levelsConfig.Levels[nextIndex].Icon );

			bool isPrevButtonActive = _selectedLevelIndex != prevIndex;
			_lobbyScreen.SetPrewButtonActive( isPrevButtonActive );
			_lobbyScreen.SetPrewGlowActive( isPrevButtonActive );

			bool isNextButtonActive = _selectedLevelIndex != nextIndex;
			_lobbyScreen.SetNextButtonActive( isNextButtonActive );
			_lobbyScreen.SetNextGlowActive( isNextButtonActive );

			_lobbyScreen.SetPrewLevelActive( _selectedLevelIndex - 1 <= _gameLevel.MaxOpened - 1 );
			_lobbyScreen.SetNextLevelActive( _selectedLevelIndex + 1 <= _gameLevel.MaxOpened - 1 );

			bool isLevelAvailable = _selectedLevelIndex <= _gameLevel.MaxOpened - 1;
			_lobbyScreen.SetPlayButtonEnabled( isLevelAvailable );
			_lobbyScreen.SetLevelActive( isLevelAvailable );
		}

		private void ResetSwitcherPosition() =>
			_lobbyScreen.SetIconNormalizedPosition( 0.5f );

		private void SetSwitcherActive ( bool value )
		{
			_lobbyScreen.SetPrewButtonActive( value );
			_lobbyScreen.SetPrewGlowActive( value );
			_lobbyScreen.SetNextButtonActive( value );
			_lobbyScreen.SetNextGlowActive( value );
		}
	}
}
