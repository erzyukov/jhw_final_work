namespace Game.Analytics
{
	using Game.Core;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using System.Collections.Generic;
	using System;

	public class CustomAnalytics : AnalyticsBase, IInitializable
	{
		[Inject] private IGameCurrency	_gameCurrency;
		[Inject] private IGameEnergy	_gameEnergy;
		[Inject] private IGameLevel		_gameLevel;

		private const int		MinuteStatInterval = 60;
		private const string	MinuteStatEventKey = "1_minute_stat";

		private int _minuteEventCount = 0;
		private int _minuteLevelStartCount = 0;
		private int _minuteWaveStartCount = 0;
		private int _minuteWinCount = 0;
		private int _minuteExitCount = 0;
		private int _minuteCoinsIncome = 0;
		private int _minuteCoinsSpent = 0;
		private int _minuteEnergySpent = 0;

		public void Initialize()
		{
			GameProfile.Analytics.SessionNumber++;
			GameProfileManager.Save();

			Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(MinuteStatInterval))
				.Subscribe(_ => OnMinuteStatRaised())
				.AddTo(this);

			_gameLevel.LevelLoaded
				.Subscribe(_ => _minuteLevelStartCount++)
				.AddTo(this);

			_gameLevel.WaveStarted
				.Subscribe(_ => _minuteWaveStartCount++)
				.AddTo(this);

			_gameLevel.LevelFinished
				.Where(r => r == GameLevel.Result.Win)
				.Subscribe(_ => _minuteWinCount++)
				.AddTo(this);

			_gameLevel.LevelFinished
				.Where(r => r == GameLevel.Result.Leave)
				.Subscribe(_ => _minuteExitCount++)
				.AddTo(this);

			_gameCurrency.SoftCurrencyTransacted
				.Subscribe(OnSoftCurrencyTransacted)
				.AddTo(this);

			_gameEnergy.EnergySpent
				.Subscribe(v => _minuteEnergySpent += v)
				.AddTo(this);
		}

		private void OnSoftCurrencyTransacted(CurrencyTransactionData<SoftTransaction> data)
		{
			if (data.Amount > 0)
				_minuteCoinsIncome += data.Amount;
			else
				_minuteCoinsSpent += data.Amount;
		}

		private void OnMinuteStatRaised()
		{
			_minuteEventCount++;

			var properties = new Dictionary<string, object>
			{
				{ "session_number",		GameProfile.Analytics.SessionNumber },
				{ "count_ads",			GameProfile.Analytics.InterWatchNumber + GameProfile.Analytics.RewardedWatchNumber },
				{ "count_int",			GameProfile.Analytics.InterWatchNumber },
				{ "count_rew",			GameProfile.Analytics.RewardedWatchNumber },
				{ "number",				_minuteEventCount },
				{ "game_start",			_minuteLevelStartCount },
				{ "wave_start",			_minuteWaveStartCount },
				{ "win",				_minuteWinCount },
				{ "exit",				_minuteExitCount },
				{ "coins_income",		_minuteCoinsIncome },
				{ "coins_spent",		_minuteCoinsSpent },
				{ "energy_used",		_minuteEnergySpent },
			};
			SendMessage(MinuteStatEventKey, properties);

			_minuteLevelStartCount = 0;
			_minuteWaveStartCount = 0;
			_minuteWinCount = 0;
			_minuteExitCount = 0;
			_minuteCoinsIncome = 0;
			_minuteCoinsSpent = 0;
			_minuteEnergySpent = 0;
		}
	}
}
