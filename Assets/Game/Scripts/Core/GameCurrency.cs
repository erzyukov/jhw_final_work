namespace Game.Core
{
	using Game.Profiles;
	using UniRx;
	using Zenject;

	public interface IGameCurrency
	{
		void AddSoftCurrency(int value);
		bool TrySpendSoftCurrency(int amount);
		void SetSummonCurrency(int value);
		void AddSummonCurrency(int value);
		bool TrySpendSummonCurrency(int amount);
	}

	public class GameCurrency : IGameCurrency
	{
		[Inject] private IGameProfileManager _gameProfileManager;

		private GameProfile GameProfile => _gameProfileManager.GameProfile;

		public void AddSoftCurrency(int value) => AddCurrency(GameProfile.SoftCurrency, value);
		public bool TrySpendSoftCurrency(int value) => TrySpendCurrency(GameProfile.SoftCurrency, value);

		public void SetSummonCurrency(int value) => GameProfile.SummonCurrency.Value = value;
		public void AddSummonCurrency(int value) => AddCurrency(GameProfile.SummonCurrency, value);
		public bool TrySpendSummonCurrency(int value) => TrySpendCurrency(GameProfile.SummonCurrency, value);

		void AddCurrency(IntReactiveProperty currency, int value)
		{
			currency.Value += value;
			Save();
		}

		bool TrySpendCurrency(IntReactiveProperty currency, int value)
		{
			if (value > currency.Value)
				return false;

			currency.Value -= value;
			Save();

			return true;
		}

		void Save() => _gameProfileManager.Save();
	}
}