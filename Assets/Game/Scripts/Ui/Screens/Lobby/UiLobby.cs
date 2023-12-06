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
		[Inject] private ILocalizator _localizator;

		private const string LevelTitlePrefixKey = "level";
		private const string lastWavePrefixKey = "uiLastWave";

		private string _levelTitlePrefix;

		public void Initialize()
		{
			_levelTitlePrefix = _localizator.GetString(LevelTitlePrefixKey);

			_lobbyScreen.Opening
				.Subscribe(_ => OnScreenOpeningHandler())
				.AddTo(this);

			_lobbyScreen.PlayButtonClicked
				.Subscribe(_ => _level.GoToLevel(_profile.LevelNumber.Value))
				.AddTo(this);
		}

		private void OnScreenOpeningHandler()
		{
			int levelIndex = _profile.LevelNumber.Value - 1;
			LevelConfig levelConfig = _levelsConfig.Levels[levelIndex];

			_lobbyScreen.SetTitle($"{_levelTitlePrefix} {levelConfig.Title}");

			_lobbyScreen.SetLastWaveActive(_profile.WaveNumber.Value != 0);
			string waveInfo = $"{_localizator.GetString(lastWavePrefixKey)} {_profile.WaveNumber.Value}/{levelConfig.Waves.Length}";
			_lobbyScreen.SetLastWaveValue(waveInfo);
		}
	}
}
