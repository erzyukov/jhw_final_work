namespace Game.Level
{
	using Game.Core;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Configs;
	using Game.Profiles;

	public class HeroSummonCurrency : ControllerBase, IInitializable
	{
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IGameCurrency _gameCurrency;
		[Inject] private CurrencyConfig _currencyConfig;
		[Inject] private GameProfile _gameProfile;

		public void Initialize()
		{
			_gameCycle.State
				.Where(state => state == GameState.TacticalStage)
				.Subscribe(_ => OnTacticalStageBeginHandler())
				.AddTo(this);
		}

		private void OnTacticalStageBeginHandler()
		{
			if (_gameProfile.WaveNumber.Value == 1)
				_gameCurrency.SetSummonCurrency(_currencyConfig.SummonCurrencyAtWaveStart);
			else
				_gameCurrency.AddSummonCurrency(_currencyConfig.SummonCurrencyAtWaveStart);
		}
	}
}