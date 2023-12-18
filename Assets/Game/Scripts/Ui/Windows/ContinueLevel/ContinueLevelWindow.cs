namespace Game
{
	using System;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;
    using TMPro;
	
	public interface IContinueLevelWindow
	{
		IObservable<Unit> NewGameButtonClicked { get; }
		IObservable<Unit> ContinueButtonClicked { get; }
		IObservable<Unit> CancelButtonClicked { get; }
		void SetActive(bool value);
        void SetNewGameButtonText(string value);
	}

	public class ContinueLevelWindow : MonoBehaviour, IContinueLevelWindow
	{
		[SerializeField] private Button _newGameButton;
		[SerializeField] private Button _continueButton;
		[SerializeField] private Button _cancelButton;
		[SerializeField] private TextMeshProUGUI _newGameButtonText;

		#region IContinueLevelWindow

		public IObservable<Unit> NewGameButtonClicked => _newGameButton.OnClickAsObservable();
		public IObservable<Unit> ContinueButtonClicked => _continueButton.OnClickAsObservable();
		public IObservable<Unit> CancelButtonClicked => _cancelButton.OnClickAsObservable();

		public void SetActive(bool value) => gameObject.SetActive(value);
        public void SetNewGameButtonText(string value) => _newGameButtonText.text = value;

        #endregion
    }
}