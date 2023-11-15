namespace Game.Ui
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Core;
	using Game.Profiles;
	using Game.Configs;

	public class UiLobby : ControllerBase, IInitializable
	{
		[Inject] private IUiLobbyScreen _lobbyScreen;
		[Inject] private IGameLevel _level;
		[Inject] private GameProfile _profile;
		[Inject] private LevelsConfig _levelsConfig;

		public void Initialize()
		{
			_lobbyScreen.Opening
				.Subscribe(_ => OnScreenOpening())
				.AddTo(this);

			_lobbyScreen.PlayButtonClicked
				.Subscribe(_ => _level.GoToLevel(_profile.LevelNumber.Value))
				.AddTo(this);
		}

		private void OnScreenOpening()
		{
			int levelIndex = _profile.LevelNumber.Value - 1;
			LevelConfig levelConfig = _levelsConfig.Levels[levelIndex];

			_lobbyScreen.SetTitle(levelConfig.Title);

			_lobbyScreen.SetLastWaveActive(_profile.WaveNumber.Value != 0);
			_lobbyScreen.SetLastWaveValue(_profile.WaveNumber.Value.ToString());
		}
	}
}
