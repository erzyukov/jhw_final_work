namespace Game.Core
{
	using Game.Profiles;
	using Game.Utilities;
	using Zenject;
	using UniRx;

	public class GameDataInitializator : ControllerBase, IInitializable
	{
		[Inject] GameProfile _gameProfile;
		[Inject] IGameProfileManager _gameProfileManager;
		[Inject] IGameCycle _gameCycle;

		public void Initialize()
		{
			_gameCycle.State
				.Where(state => state == GameState.Lobby)
				.Subscribe(_ => OnLobbyLoadedHandler())
				.AddTo(this);
		}

		private void OnLobbyLoadedHandler()
		{
			if (_gameProfile.WaveNumber.Value == 1)
				_gameProfile.WaveNumber.Value = 0;

			if (_gameProfile.WaveNumber.Value == 0)
				_gameProfile.HeroField.Units.Clear();

			_gameProfileManager.Save();
		}
	}
}