namespace Game.Ui
{
	using System;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public interface IUiGameSettingsWindow : IUiWindow
	{
		IObservable<bool> MusicToggleChanged { get; }
		IObservable<bool> SoundToggleChanged { get; }
		void SetMusicActive(bool value);
		void SetSoundActive(bool value);
	}

	public class UiGameSettingsWindow : UiWindow, IUiGameSettingsWindow
	{
		[SerializeField] private Toggle _musicToggle;
		[SerializeField] private Toggle _soundToggle;

		#region IContinueLevelWindow
		public override Window Type => Window.GameSettings;

		public IObservable<bool> MusicToggleChanged => _musicToggle.OnValueChangedAsObservable();
		public IObservable<bool> SoundToggleChanged => _soundToggle.OnValueChangedAsObservable();

		public void SetMusicActive(bool value) =>
			_musicToggle.isOn = value;

		public void SetSoundActive(bool value) =>
			_soundToggle.isOn = value;

		#endregion
	}
}