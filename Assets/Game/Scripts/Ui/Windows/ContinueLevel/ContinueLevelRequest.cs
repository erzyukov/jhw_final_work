namespace Game.Ui
{
    using Game.Configs;
    using Game.Core;
    using Game.Utilities;
    using System;
	using UniRx;
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
        private const string pricePrefixKey = "energyPriceValue";
        private const string valueTemplate = "{{}}";

        private Action _currentNewGameAction;
		private Action _currentContinueAction;

		public void Initialize()
		{
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

			string price = _localizator.GetString(pricePrefixKey).Replace(valueTemplate, _energyConfig.LevelPrice.ToString());
			string newGameButtonTitle = _localizator.GetString(newGameKey) + "\n" + price;
            _window.SetNewGameButtonText(newGameButtonTitle);

            _window.SetActive(true);
		}

		#endregion

		private void NewGameButtonClickedHandler()
		{
			_window.SetActive(false);
			_currentNewGameAction?.Invoke();
		}

		private void ContinueButtonClickedHandler()
		{
			_window.SetActive(false);
			_currentContinueAction?.Invoke();
		}
	}
}
