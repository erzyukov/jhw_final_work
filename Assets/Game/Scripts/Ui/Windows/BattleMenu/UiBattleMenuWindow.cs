namespace Game.Ui
{
	using System;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public interface IUiBattleMenuWindow : IUiWindow
	{
		IObservable<Unit> LobbyButtonClicked { get; }
		IObservable<Unit> ContinueButtonClicked { get; }
	}

	public class UiBattleMenuWindow : UiWindow, IUiBattleMenuWindow
	{
		[SerializeField] private Button _lobbyButton;
		[SerializeField] private Button _continueButton;

		#region IContinueLevelWindow

		public override Window Type => Window.BattleMenu;

		public IObservable<Unit> LobbyButtonClicked => _lobbyButton.OnClickAsObservable();
		public IObservable<Unit> ContinueButtonClicked => _continueButton.OnClickAsObservable();

		#endregion
	}
}