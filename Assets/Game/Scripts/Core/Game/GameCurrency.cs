namespace Game.Core
{
	using Game.Profiles;
	using UniRx;
	using Zenject;

	// TODO: Add summon currency message
	// TODO: Add summon currency animate
	public enum SoftTransaction
	{
		None,
		Upgrade,
		LevelFinishReward,
		HeroLevelReward,
	}

	public struct CurrencyTransactionData<T> where T : System.Enum
	{
		public T Type;
		public string Detail;
		public int Amount;
	}

	public interface IGameCurrency
	{
		ReactiveCommand<CurrencyTransactionData<SoftTransaction>> SoftCurrencyTransacted { get; }
		void AddSoftCurrency(int value, SoftTransaction type, string detail);
		bool TrySpendSoftCurrency(int amount, SoftTransaction type, string detail);
		void SetSummonCurrency(int value);
		void AddSummonCurrency(int value);
		bool TrySpendSummonCurrency(int amount);
		void AddLevelSoftCurrency(int value);
		void ResetLevelSoftCurrency();
		void ConsumeLevelSoftCurrency(string detail);
	}

	public class GameCurrency : IGameCurrency
	{
		[Inject] private IGameProfileManager _gameProfileManager;

		private GameProfile GameProfile => _gameProfileManager.GameProfile;

		#region IGameCurrency

		public ReactiveCommand<CurrencyTransactionData<SoftTransaction>> SoftCurrencyTransacted { get; } = new ReactiveCommand<CurrencyTransactionData<SoftTransaction>>();

		public void AddSoftCurrency(int value, SoftTransaction type, string detail)
		{
			AddCurrency(GameProfile.SoftCurrency, value);
			RegisterTransaction(value, type, detail);
		}

		public bool TrySpendSoftCurrency(int value, SoftTransaction type, string detail)
		{
			bool result = TrySpendCurrency(GameProfile.SoftCurrency, value);

			if (result)
				RegisterTransaction(-value, type, detail);

			return result;
		}

		public void SetSummonCurrency(int value) => 
			GameProfile.SummonCurrency.Value = value;

		public void AddSummonCurrency(int value) => 
			AddCurrency(GameProfile.SummonCurrency, value);

		public bool TrySpendSummonCurrency(int value) => 
			TrySpendCurrency(GameProfile.SummonCurrency, value);

		public void AddLevelSoftCurrency(int value) =>
			AddCurrency(GameProfile.LevelSoftCurrency, value);

		public void ResetLevelSoftCurrency() =>
			GameProfile.LevelSoftCurrency.Value = 0;

		public void ConsumeLevelSoftCurrency(string detail)
		{
			AddSoftCurrency(GameProfile.LevelSoftCurrency.Value, SoftTransaction.LevelFinishReward, detail.ToString());
			ResetLevelSoftCurrency();
			Save();
		}

		#endregion

		private void AddCurrency(IntReactiveProperty currency, int value)
		{
			currency.Value += value;
			Save();
		}

		private bool TrySpendCurrency(IntReactiveProperty currency, int value)
		{
			if (value > currency.Value)
				return false;

			currency.Value -= value;
			Save();

			return true;
		}

		private void Save() => _gameProfileManager.Save(true);

		public void RegisterTransaction(int amount, SoftTransaction type, string detail)
		{
			var data = new CurrencyTransactionData<SoftTransaction>
			{
				Type = type,
				Detail = detail,
				Amount = amount
			};

			SoftCurrencyTransacted.Execute(data);
		}

	}
}