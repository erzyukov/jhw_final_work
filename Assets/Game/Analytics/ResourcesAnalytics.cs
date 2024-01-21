namespace Game.Analytics
{
	using Game.Core;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using System.Collections.Generic;
	using Game.Profiles;
	using static Core.GameCurrency;

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
			_gameCurrency.SoftCurrencyMoved
				.Subscribe(OnSoftCurrencyMoved)
				.AddTo(this);
		}

		private void OnSoftCurrencyMoved(CurrencyMovement<Soft> movement)
		{
			var properties = new Dictionary<string, object>
			{
				{ "category", movement.Type },
				{ "item_id", $"{movement.Type}_{movement.Id}" },
				{ "value", movement.Amount.ToString() },
				{ "soft_balance", _gameProfile.SoftCurrency.Value },
				{ "level_number", _gameProfile.LevelNumber.Value },
			};
			_eventSender.SendMessage(SoftCurrencyEventKey, properties);
		}
	}
}
