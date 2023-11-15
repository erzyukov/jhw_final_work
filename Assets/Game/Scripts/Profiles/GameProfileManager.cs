namespace Game.Profiles
{
	public interface IGameProfileManager
	{
		GameProfile GameProfile { get; }
		void Save();
	}

	public class GameProfileManager : IGameProfileManager
	{
		private GameProfile _gameProfile;

		public void OnInstantiated()
		{
			_gameProfile = new GameProfile();
		}

		public GameProfile GameProfile => _gameProfile;

		public void Save()
		{
			// TODO: save game profile
		}
	}
}