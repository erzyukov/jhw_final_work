namespace Game.Ui
{
	using UnityEngine;
	using TMPro;
	using UnityEngine.UI;
	using System;
	using UniRx;

	public interface IUiLobbyScreen : IUiScreen
	{
		IObservable<Unit> PlayButtonClicked { get; }
		void SetTitle(string value);
		void SetLastWaveValue(string value);
		void SetLastWaveActive(bool value);
        void SetPlayButtonText(string value);
		void SetPlayButtonEnabled(bool value);
	}

	public class UiLobbyScreen : UiScreen, IUiLobbyScreen
	{
		[SerializeField] private TextMeshProUGUI _levelTitle;
		[SerializeField] private GameObject _lastWave;
		[SerializeField] private TextMeshProUGUI _lastWaveValue;
		[SerializeField] private Button _playButton;
		[SerializeField] private TextMeshProUGUI _playButtonText;

		public override Screen Screen => Screen.Lobby;

		#region IUiLobbyScreen

		public IObservable<Unit> PlayButtonClicked => _playButton.OnClickAsObservable();
		
		public void SetTitle(string value) => _levelTitle.text = value;

		public void SetLastWaveValue(string value) => _lastWaveValue.text = value;

		public void SetLastWaveActive(bool value) => _lastWave.SetActive(value);

        public void SetPlayButtonText(string value) => _playButtonText.text = value;

		public void SetPlayButtonEnabled(bool value) => _playButton.interactable = value;

		#endregion
	}
}
