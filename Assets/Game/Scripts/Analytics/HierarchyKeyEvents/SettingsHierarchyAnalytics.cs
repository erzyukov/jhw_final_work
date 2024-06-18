namespace Game.Analytics
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Profiles;
	using UnityEngine;

	public class SettingsHierarchyAnalytics : HierarchyAnalyticsBase, IInitializable
	{
		[Inject] private GameProfile _gameProfile;

		private const string SettingsEventKey = "Settings";
		private const string MusicSettingsEventKey = "Music";
		private const string SoundSettingsEventKey = "Sound";

		private const string OnValue = "On";
		private const string OffValue = "Off";

		public void Initialize()
		{
			_gameProfile.IsMusicEnabled
				.Skip(1)
				.Subscribe(OnMusicSettingsChanged)
				.AddTo(this);
			
			_gameProfile.IsSoundEnabled
				.Skip(1)
				.Subscribe(OnSoundSettingsChanged)
				.AddTo(this);
		}

		private void OnMusicSettingsChanged(bool value)
		{
			//SendDesignEvent($"{SettingsEventKey}:{MusicSettingsEventKey}:{GetToggleString(value)}", Time.time);
		}

		private void OnSoundSettingsChanged(bool value)
		{
			//SendDesignEvent($"{SettingsEventKey}:{SoundSettingsEventKey}:{GetToggleString(value)}", Time.time);
		}

		private string GetToggleString(bool value) =>
			value ? OnValue : OffValue;
	}
}