namespace Game.Profiles
{
	public interface IGameProfileManager
	{
		GameProfile GameProfile { get; }
	}

	public class GameProfileManager : IGameProfileManager
	{
		private GameProfile _gameProfile;

		public GameProfile GameProfile => _gameProfile;

		public void OnInstantiated()
		{
			_gameProfile = new GameProfile();
		}
	}
}