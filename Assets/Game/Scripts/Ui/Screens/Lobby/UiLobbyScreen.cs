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
        void SetPlayPriceText(string value);
		void SetPlayPriceActive(bool value);
		void SetPlayButtonEnabled(bool value);
	}

	public class UiLobbyScreen : UiScreen, IUiLobbyScreen
	{
		[SerializeField] private TextMeshProUGUI _levelTitle;
		[SerializeField] private GameObject _lastWave;
		[SerializeField] private TextMeshProUGUI _lastWaveValue;
		[SerializeField] private Button _playButton;
		[SerializeField] private GameObject _playIcon;
		[SerializeField] private GameObject _playPrice;
		[SerializeField] private TextMeshProUGUI _playPriceText;

		public override Screen Screen => Screen.Lobby;

		#region IUiLobbyScreen

		public IObservable<Unit> PlayButtonClicked => _playButton.OnClickAsObservable();
		
		public void SetTitle(string value) => _levelTitle.text = value;

		public void SetLastWaveValue(string value) => _lastWaveValue.text = value;

		public void SetLastWaveActive(bool value) => _lastWave.SetActive(value);

		public void SetPlayPriceActive(bool value)
		{
			_playIcon.SetActive(!value);
			_playPrice.SetActive(value);
		}

		public void SetPlayPriceText(string value) => _playPriceText.text = value;

		public void SetPlayButtonEnabled(bool value) => _playButton.interactable = value;

		#endregion
	}
}
