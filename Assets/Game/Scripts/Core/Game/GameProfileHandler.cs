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
		[Inject] private CurrencyConfig _currencyConfig;

		private bool _isWaveFirstInBattle;

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
			_isWaveFirstInBattle = true;

			if (_gameProfile.WaveNumber.Value == 0)
			{
				_gameProfile.HeroField.Units.Clear();
				_gameCurrency.SetSummonCurrency(_currencyConfig.SummonCurrencyAtWaveStart);
				_gameProfileManager.Save();
			}
		}

		private void OnTacticalStageBeginHandler()
		{
			if (_isWaveFirstInBattle)
				_isWaveFirstInBattle = false;
			else
				_gameCurrency.AddSummonCurrency(_currencyConfig.SummonCurrencyAtWaveStart);
		}

		private void OnBattleFinishHandler()
		{
			_gameProfile.WaveNumber.Value = 0;
			_gameProfile.HeroField.Units.Clear();
			_gameProfileManager.Save();
		}
	}
}