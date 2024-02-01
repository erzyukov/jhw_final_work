namespace Game.Ui
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Profiles;

	public class UiGameSettings : ControllerBase, IInitializable
	{
		[Inject] private IUiGameSettingsWindow _window;
		[Inject] private GameProfile _gameProfile;
		[Inject] private IGameProfileManager _gameProfileManager;

		public void Initialize()
		{
			_window.SetMusicActive(_gameProfile.IsMusicEnabled.Value);
			_window.SetSoundActive(_gameProfile.IsSoundEnabled.Value);

			_window.CancelButtonClicked
				.Subscribe(_ => _window.SetActive(false))
				.AddTo(this);

			_window.MusicToggleChanged
				.Subscribe(OnMusicToggleChanged)
				.AddTo(this);

			_window.SoundToggleChanged
				.Subscribe(OnSoundToggleChanged)
				.AddTo(this);
		}

		private void OnMusicToggleChanged(bool value)
		{
			_gameProfile.IsMusicEnabled.Value = value;
			_gameProfileManager.Save();
		}

		private void OnSoundToggleChanged(bool value)
		{
			_gameProfile.IsSoundEnabled.Value = value;
			_gameProfileManager.Save();
		}
	}
}