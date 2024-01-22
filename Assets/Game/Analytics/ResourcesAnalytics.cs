namespace Game.Analytics
{
	using Game.Core;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using System.Collections.Generic;
	using Game.Profiles;

	public class ResourcesAnalytics : ControllerBase, IInitializable
	{
		[Inject] private IAnalyticEventSender _eventSender;
		[Inject] private IGameCurrency _gameCurrency;
		[Inject] private GameProfile _gameProfile;

		private const string SoftCurrencyEventKey = "soft_currency";
		private const string PlayerExperienceEventKey = "player_xp";
		private const string EnergyEventKey = "energy_used";

		public void Initialize()
		{
			_gameCurrency.SoftCurrencyTransacted
				.Subscribe(OnSoftCurrencyMoved)
				.AddTo(this);
		}

		private void OnSoftCurrencyMoved(CurrencyTransactionData<SoftTransaction> data)
		{
			var properties = new Dictionary<string, object>
			{
				{ "category", data.Type },
				{ "item_id", $"{data.Type}_{data.Detail}" },
				{ "value", data.Amount.ToString() },
				{ "soft_balance", _gameProfile.SoftCurrency.Value },
				{ "level_number", _gameProfile.LevelNumber.Value },
			};
			_eventSender.SendMessage(SoftCurrencyEventKey, properties);
		}
	}
}
