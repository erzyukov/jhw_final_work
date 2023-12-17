namespace Game.Ui
{
    using Game.Configs;
    using Game.Core;
    using Game.Utilities;
    using System;
	using UniRx;
    using UnityEngine;
    using Zenject;

	public interface IContinueLevelRequest
	{
		void ShowRequest(Action newGameCallback, Action continueCallback);
	}

	public class ContinueLevelRequest : ControllerBase, IContinueLevelRequest, IInitializable
	{
		[Inject] private IContinueLevelWindow _window;
        [Inject] private EnergyConfig _energyConfig;
        [Inject] private ILocalizator _localizator;

        private const string newGameKey = "newGame";
        private const string pricePrefixKey = "pricePrefix";

        private Action _currentNewGameAction;
		private Action _currentContinueAction;

		public void Initialize()
		{
			_window.CancelButtonClicked
				.Subscribe(_ => CloseWindow())
				.AddTo(this);

			_window.ContinueButtonClicked
				.Subscribe(_ => ContinueButtonClickedHandler())
				.AddTo(this);

			_window.NewGameButtonClicked
				.Subscribe(_ => NewGameButtonClickedHandler())
				.AddTo(this);
		}

		#region IContinueLevelRequest

		public void ShowRequest(Action newGameCallback, Action continueCallback)
		{
			_currentNewGameAction = newGameCallback;
			_currentContinueAction = continueCallback;

            string newGameButtonTitle = _localizator.GetString(newGameKey)
                + $"\n{_localizator.GetString(pricePrefixKey)}{_energyConfig.LevelPrice}";
            _window.SetNewGameButtonText(newGameButtonTitle);

            _window.SetActive(true);
		}

		#endregion

		private void CloseWindow()
		{
			_window.SetActive(false);
		}

		private void NewGameButtonClickedHandler()
		{
			CloseWindow();
			_currentNewGameAction?.Invoke();
		}

		private void ContinueButtonClickedHandler()
		{
			CloseWindow();
			_currentContinueAction?.Invoke();
		}
	}
}
