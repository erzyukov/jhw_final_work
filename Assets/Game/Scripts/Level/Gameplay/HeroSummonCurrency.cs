namespace Game.Level
{
	using Game.Core;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Configs;

	public class HeroSummonCurrency : ControllerBase, IInitializable
	{
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IGameCurrency _gameCurrency;
		[Inject] private CurrencyConfig _currencyConfig;

		public void Initialize()
		{
			_gameCycle.State
				.Where(state => state == GameState.TacticalStage)
				.Subscribe(_ => OnTacticalStageBeginHandler())
				.AddTo(this);
		}

		private void OnTacticalStageBeginHandler()
		{
			_gameCurrency.SetSummonCurrency(_currencyConfig.SummonCurrencyAtWaveStart);
		}
	}
}
