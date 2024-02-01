namespace Game.Ui
{
	using System;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public interface IUiGameSettingsWindow
	{
		IObservable<bool> MusicToggleChanged { get; }
		IObservable<bool> SoundToggleChanged { get; }
		IObservable<Unit> CancelButtonClicked { get; }
		void SetActive(bool value);
		void SetMusicActive(bool value);
		void SetSoundActive(bool value);
	}

	public class UiGameSettingsWindow : MonoBehaviour, IUiGameSettingsWindow
	{
		[SerializeField] private Toggle _musicToggle;
		[SerializeField] private Toggle _soundToggle;
		[SerializeField] private Button _cancelButton;

		#region IContinueLevelWindow

		public IObservable<bool> MusicToggleChanged => _musicToggle.OnValueChangedAsObservable();
		public IObservable<bool> SoundToggleChanged => _soundToggle.OnValueChangedAsObservable();
		public IObservable<Unit> CancelButtonClicked => _cancelButton.OnClickAsObservable();

		public void SetActive(bool value) => 
			gameObject.SetActive(value);

		public void SetMusicActive(bool value) =>
			_musicToggle.isOn = value;

		public void SetSoundActive(bool value) =>
			_soundToggle.isOn = value;

		#endregion
	}
}