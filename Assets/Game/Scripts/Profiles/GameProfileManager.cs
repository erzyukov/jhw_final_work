namespace Game.Profiles
{
	public interface IGameProfileManager
	{
		void Initialize();
		GameProfile GameProfile { get; }
	}

	public class GameProfileManager : IGameProfileManager
	{
		private GameProfile _gameProfile;

		public GameProfile GameProfile => _gameProfile;

		public void Initialize()
		{
			_gameProfile = new GameProfile();
		}
	}
}