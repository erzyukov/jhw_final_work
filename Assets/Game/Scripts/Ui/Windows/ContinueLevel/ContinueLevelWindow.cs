namespace Game.Ui
{
	using System;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;
    using TMPro;
	
	public interface IContinueLevelWindow : IUiWindow
	{
		IObservable<Unit> NewGameButtonClicked { get; }
		IObservable<Unit> ContinueButtonClicked { get; }
        void SetNewGameButtonText(string value);
	}

	public class ContinueLevelWindow : UiWindow, IContinueLevelWindow
	{
		[SerializeField] private Button _newGameButton;
		[SerializeField] private Button _continueButton;
		[SerializeField] private TextMeshProUGUI _newGameButtonText;

		#region IContinueLevelWindow

		public override Window Type => Window.ContinueLevel;

		public IObservable<Unit> NewGameButtonClicked => _newGameButton.OnClickAsObservable();
		public IObservable<Unit> ContinueButtonClicked => _continueButton.OnClickAsObservable();
        public void SetNewGameButtonText(string value) => _newGameButtonText.text = value;

        #endregion
    }
}