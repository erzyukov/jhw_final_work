namespace Game.Core
{
	using Game.Profiles;
	using Zenject;

	public interface IGameCurrency
	{
		void AddSoftCurrency(int value);
		bool TrySpendSoftCurrency(int amount);
	}

	public class GameCurrency : IGameCurrency
	{
		[Inject] private IGameProfileManager _gameProfileManager;

		private GameProfile GameProfile => _gameProfileManager.GameProfile;

		public void AddSoftCurrency(int value)
		{
			GameProfile.SoftCurrency.Value += value;
			
			Save();
		}

		public bool TrySpendSoftCurrency(int value)
		{
			if (value > GameProfile.SoftCurrency.Value)
				return false;

			GameProfile.SoftCurrency.Value -= value;
			Save();

			return true;
		}

		void Save() => _gameProfileManager.Save();
	}
}