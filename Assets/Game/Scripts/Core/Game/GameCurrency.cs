namespace Game.Core
{
	using Game.Profiles;
	using UniRx;
	using Zenject;
	using static GameCurrency;

	// TODO: Add summon currency message
	// TODO: Add summon currency animate

	public interface IGameCurrency
	{
		ReactiveCommand<CurrencyMovement<Soft>> SoftCurrencyMoved { get; }
		void AddSoftCurrency(int value, Soft type, string id);
		bool TrySpendSoftCurrency(int amount, Soft type, string id);
		void SetSummonCurrency(int value);
		void AddSummonCurrency(int value);
		bool TrySpendSummonCurrency(int amount);
		void AddLevelSoftCurrency(int value);
		void ResetLevelSoftCurrency();
		void ConsumeLevelSoftCurrency();
	}

	public class GameCurrency : IGameCurrency
	{
		[Inject] private IGameProfileManager _gameProfileManager;

		public enum Soft
		{
			None,
			Upgrade,
			LevelFinishReward,
			HeroLevelReward,
		}

		private GameProfile GameProfile => _gameProfileManager.GameProfile;

		#region IGameCurrency

		public ReactiveCommand<CurrencyMovement<Soft>> SoftCurrencyMoved { get; } = new ReactiveCommand<CurrencyMovement<Soft>>();

		public void AddSoftCurrency(int value, Soft type, string id)
		{
			AddCurrency(GameProfile.SoftCurrency, value);

			SoftCurrencyMoved.Execute(new CurrencyMovement<Soft>
			{
				Type = type,
				Id = id,
				Amount = value
			});
		}

		public bool TrySpendSoftCurrency(int value, Soft type, string id)
		{
			bool canSpend = TrySpendCurrency(GameProfile.SoftCurrency, value);

			if (canSpend)
			{
				SoftCurrencyMoved.Execute(new CurrencyMovement<Soft>
				{
					Type = type,
					Id = id,
					Amount = -value
				});
			}

			return canSpend;
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

		public void ConsumeLevelSoftCurrency()
		{
			AddSoftCurrency(GameProfile.LevelSoftCurrency.Value, Soft.LevelFinishReward, GameProfile.IsWonLastBattle ? "win": "fail");
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

		private void Save() => _gameProfileManager.Save();

		public struct CurrencyMovement<T> where T : System.Enum
		{
			public T Type;
			public string Id;
			public int Amount;
		}
	}
}