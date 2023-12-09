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

			_gameCycle.State
				.Where(state => state == GameState.WinBattle || state == GameState.LoseBattle)
				.Subscribe(_ => OnBattleFinishHandler())
				.AddTo(this);
		}

		private void OnLevelLoadingHandler()
		{
			_levelConfig = _levelsConfig.Levels[_gameProfile.LevelNumber.Value - 1];
			_isWaveFirstInBattle = true;

			if (_gameProfile.WaveNumber.Value == 0)
			{
				int waveIndex = _gameProfile.WaveNumber.Value;
				WaveConfig waveConfig = _levelConfig.Waves[waveIndex];
				_gameProfile.HeroField.Units.Clear();
				_gameCurrency.SetSummonCurrency(waveConfig.SummonCurrencyAmount);
				_gameProfileManager.Save();
			}
		}

		private void OnTacticalStageBeginHandler()
		{
			if (_isWaveFirstInBattle)
			{
				_isWaveFirstInBattle = false;
			}
			else
			{
				int waveIndex = _gameProfile.WaveNumber.Value - 1;
				WaveConfig waveConfig = _levelConfig.Waves[waveIndex];
				_gameCurrency.AddSummonCurrency(waveConfig.SummonCurrencyAmount);
			}
		}

		private void OnBattleFinishHandler()
		{
			_gameProfile.WaveNumber.Value = 0;
			_gameProfile.HeroField.Units.Clear();
			_gameProfileManager.Save();
		}
	}
}