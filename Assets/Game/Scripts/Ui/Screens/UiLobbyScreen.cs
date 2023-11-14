namespace Game.Ui
{
	using UnityEngine;
	using TMPro;
	using UnityEngine.UI;
	using System;
	using UniRx;

	public interface IUiLobbyScreen
	{
		void SetTitle(string value);
		void SetLastWaveTitle(string value);
		IObservable<Unit> PlayButtonClicked { get; }
	}

	public class UiLobbyScreen : UiScreen, IUiLobbyScreen
	{
		[SerializeField] private TextMeshProUGUI _levelTitle;
		[SerializeField] private TextMeshProUGUI _lastWaveTitle;
		[SerializeField] private Button _playButton;

		public override Screen Screen => Screen.Lobby;

		#region IUiLobbyScreen

		public IObservable<Unit> PlayButtonClicked => _playButton.OnClickAsObservable();
		
		public void SetTitle(string value) => _levelTitle.text = value;

		public void SetLastWaveTitle(string value) => _lastWaveTitle.text = value;
		
		#endregion
	}
}
