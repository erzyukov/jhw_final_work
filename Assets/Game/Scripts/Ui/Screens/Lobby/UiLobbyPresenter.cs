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
	using Newtonsoft.Json.Linq;

	public class UiLobbyPresenter : ControllerBase, IInitializable
	{
		[Inject] private IUiLobbyScreen _lobbyScreen;
		[Inject] private IUiLobbyFlow _flow;
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
		private int SelectedLevelIndex => _flow.SelectedLevelIndex.Value;

		public void Initialize()
		{
			_levelTitlePrefix = _localizator.GetString( LevelTitlePrefixKey );
			_flow.SelectedLevelIndex.Value = _profile.LevelNumber.Value - 1;
			_flow.IsSelectLevelAvailable.Value = true;

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

			_flow.IsStartAvailable
				.Subscribe( OnStartButtonAvailable )
				.AddTo( this );

			_flow.IsSelectLevelAvailable
				.Subscribe( SetSwitcherActive )
				.AddTo( this );

			_flow.IsNextStageAvailable
				.Subscribe( _lobbyScreen.SetNextLevelAnimationActive )
				.AddTo( this );

			_flow.SelectedLevelIndex
				.Subscribe( i => _flow.IsNextStageAvailable.Value = i + 1 < _gameLevel.MaxOpened )
				.AddTo( this );
		}

		private void OnScreenOpeningHandler()
		{
			_flow.IsNextStageAvailable.Value = _profile.LevelNumber.Value < _gameLevel.MaxOpened;

			_flow.SelectedLevelIndex.Value = _profile.LevelNumber.Value - 1;

			SetLevelInfo( SelectedLevelIndex );

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
			( SelectedLevelIndex == _profile.LevelNumber.Value - 1 )
				? _profile.WaveNumber.Value
				: 0;

		private void OnPlayButtonClickedHandler()
		{
			_flow.PlayButtonClicked.Execute();

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

				_gameLevel.GoToLevel( SelectedLevelIndex + 1, targetWave );
			}
			else
			{
				_uiMessage.ShowMessage( UiMessage.NotEnoughEnergy );

				_resourceEvents.LowEnergyAlert.Execute();
			}
		}

		private void GoToLevelFree() =>
			_gameLevel.GoToLevel( SelectedLevelIndex + 1, WaveNumber );

		private void OnPreviousLevelClicked() =>
			AnimateSwitching( 0.5f, 0, () => SetSelectedPrew() );

		private void OnNextLevelClicked() =>
			AnimateSwitching( 0.5f, 1, () => SetSelectedNext() );

		private void SetSelectedPrew() =>
			_flow.SelectedLevelIndex.Value = Mathf.Max( 0, SelectedLevelIndex - 1 );

		private void SetSelectedNext() =>
			_flow.SelectedLevelIndex.Value = Mathf.Min( _levelsConfig.Levels.Length - 1, SelectedLevelIndex + 1 );

		private void AnimateSwitching(float from, float to, Action callback)
		{
			_flow.IsSelectLevelAvailable.Value = false;
			_flow.IsStartAvailable.Value = false;

			DOVirtual.Float( from, to, SwitchDuration, ( t ) =>
			{
				_lobbyScreen.SetIconNormalizedPosition( t );
			} ).SetEase( Ease.InOutCubic )
			.OnComplete(() =>
			{
				_flow.IsSelectLevelAvailable.Value = true;
				callback.Invoke();
				SetLevelInfo( SelectedLevelIndex );
				UpdateScreenState();
				ResetSwitcherPosition();
			} );
		}

		private void UpdateScreenState()
		{
			int prevIndex = Mathf.Max( 0 , SelectedLevelIndex - 1);
			int nextIndex = Mathf.Min( _levelsConfig.Levels.Length - 1, SelectedLevelIndex + 1 );

			_lobbyScreen.SetPrewLevelIcon( _levelsConfig.Levels[prevIndex].Icon );
			_lobbyScreen.SetNextLevelIcon( _levelsConfig.Levels[nextIndex].Icon );

			bool isPrevButtonActive = SelectedLevelIndex != prevIndex;
			SetPrevButtonActive( isPrevButtonActive );

			bool isNextButtonActive = SelectedLevelIndex != nextIndex;
			SetNextButtonActive( isNextButtonActive );

			_lobbyScreen.SetPrewLevelActive( SelectedLevelIndex - 1 <= _gameLevel.MaxOpened - 1 );
			_lobbyScreen.SetNextLevelActive( SelectedLevelIndex + 1 <= _gameLevel.MaxOpened - 1 );

			bool isLevelAvailable = SelectedLevelIndex <= _gameLevel.MaxOpened - 1;
			_flow.IsStartAvailable.Value = isLevelAvailable;
			_lobbyScreen.SetLevelActive( isLevelAvailable );
		}

		private void ResetSwitcherPosition() =>
			_lobbyScreen.SetIconNormalizedPosition( 0.5f );

		private void SetSwitcherActive ( bool value )
		{
			SetPrevButtonActive( value );
			SetNextButtonActive( value );

			if (value == true)
				UpdateScreenState();
		}

		private void SetPrevButtonActive( bool value )
		{
			value &= _flow.IsSelectLevelAvailable.Value;

			_lobbyScreen.SetPrevButtonActive( value );
			_lobbyScreen.SetPrevGlowActive( value );
		}

		private void SetNextButtonActive( bool value )
		{
			value &= _flow.IsSelectLevelAvailable.Value;

			_lobbyScreen.SetNextButtonActive( value );
			_lobbyScreen.SetNextGlowActive( value );
		}

		private void OnStartButtonAvailable( bool value )
		{
			_lobbyScreen.SetPlayButtonEnabled( value );
		}
	}
}
