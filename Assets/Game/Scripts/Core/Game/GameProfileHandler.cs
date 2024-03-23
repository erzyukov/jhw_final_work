namespace Game.Core
{
	using Game.Profiles;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Configs;

	public class GameProfileHandler : ControllerBase, IInitializable
	{
		[Inject] private GameProfile _gameProfile;
		[Inject] private IGameProfileManager _gameProfileManager;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IGameLevel _gameLevel;
		[Inject] private IGameCurrency _gameCurrency;
		[Inject] private LevelsConfig _levelsConfig;
		[Inject] private CommonGameplayConfig _commonConfig;

		private bool _isWaveFirstInBattle;
		private LevelConfig _levelConfig;

		public void Initialize()
		{
			_gameLevel.LevelLoading
				.Subscribe(_ => OnLevelLoadingHandler())
				.AddTo(this);

			_gameCycle.State
				.Where(state => state == GameState.TacticalStage)
				.Subscribe(_ => OnTacticalStageBeginHandler())
				.AddTo(this);

			Observable.Merge(
				_gameCycle.State.Where(state => state == GameState.WinBattle).AsUnitObservable(),
				_gameLevel.LevelClosing
			)
				.Subscribe(_ => OnBattleFinishHandler())
				.AddTo(this);

			_gameCycle.State
				.Where(state => state == GameState.Lobby)
				.Subscribe(_ => OnLobbyLoadedHandler())
				.AddTo(this);
		}

		private void OnLevelLoadingHandler()
		{
			_levelConfig = _levelsConfig.Levels[_gameProfile.LevelNumber.Value - 1];
			_isWaveFirstInBattle = true;

			if (_gameProfile.WaveNumber.Value == 0)
			{
				int tokenAmount = _gameLevel.GetCurrentWaveSummonCurrency();

				_gameProfile.ReviveAttemptsCount = _commonConfig.DefaultReviveAttempts;
				_gameProfile.HeroField.Units.Clear();
				_gameCurrency.SetSummonCurrency(tokenAmount);
				_gameProfileManager.Save();
			}

			ResetFirstAppRun();
		}

		private void OnTacticalStageBeginHandler()
		{
			if (_isWaveFirstInBattle)
			{
				_isWaveFirstInBattle = false;
			}
			else
			{
				int tokenAmount = _gameLevel.GetCurrentWaveSummonCurrency();
				_gameCurrency.AddSummonCurrency(tokenAmount);
			}
		}

		private void OnBattleFinishHandler()
		{
			_gameProfile.WaveNumber.Value = 0;
			_gameProfile.HeroField.Units.Clear();
			_gameProfile.IsReturnFromBattle = true;
			_gameProfileManager.Save();
		}

		private void OnLobbyLoadedHandler()
		{
			if (_gameProfile.IsReturnFromBattle)
			{
				_gameProfile.IsReturnFromBattle = false;

				_gameCurrency.ConsumeLevelSoftCurrency(_gameProfile.IsWonLastBattle ? "win" : "fail");
			}

			ResetFirstAppRun();
		}

		private void ResetFirstAppRun()
		{
			if (_gameProfile.Analytics.IsFirstRunApp)
			{
				_gameProfile.Analytics.IsFirstRunApp = false;
				_gameProfileManager.Save();
			}
		}
	}
}