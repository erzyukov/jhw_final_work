namespace Game.Ui
{
	using System;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public interface IUiBattleMenuWindow
	{
		IObservable<Unit> LobbyButtonClicked { get; }
		IObservable<Unit> ContinueButtonClicked { get; }
		IObservable<Unit> CancelButtonClicked { get; }
		void SetActive(bool value);
	}

	public class UiBattleMenuWindow : MonoBehaviour, IUiBattleMenuWindow
	{
		[SerializeField] private Button _lobbyButton;
		[SerializeField] private Button _continueButton;
		[SerializeField] private Button _cancelButton;

		#region IContinueLevelWindow

		public IObservable<Unit> LobbyButtonClicked => _lobbyButton.OnClickAsObservable();
		public IObservable<Unit> ContinueButtonClicked => _continueButton.OnClickAsObservable();
		public IObservable<Unit> CancelButtonClicked => _cancelButton.OnClickAsObservable();

		public void SetActive(bool value) => gameObject.SetActive(value);

		#endregion
	}
}