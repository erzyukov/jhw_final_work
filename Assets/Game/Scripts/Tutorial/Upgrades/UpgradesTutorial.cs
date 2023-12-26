namespace Game.Tutorial
{
    using Game.Profiles;
    using Game.Utilities;
    using Zenject;
    using UniRx;
    using System;
    using Game.Core;
    using Game.Configs;
    using System.Linq;
    using Game.Ui;
	using Game.Units;
	using static Game.Configs.TutorialConfig;
	using UnityEngine;

	public class UpgradesTutorial : UpgradesTutorialFsmBase, IInitializable, IDisposable
	{
		[Inject] private GameProfile _profile;
		[Inject] private IGameProfileManager _gameProfileManager;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IGameLevel _gameLevel;
		[Inject] private IGameUpgrades _gameUpgrades;
		[Inject] private ILocalizator _localizator;
		[Inject] private MenuConfig _menuConfig;
		[Inject] private TutorialConfig _config;
		[Inject] private IFingerHint _fingerHint;
		[Inject] private IDialogHint _dialogHint;
		[Inject] private IUiMainMenuView _uiMainMenuView;
		[Inject] private IUiLobbyScreen _uiLobbyScreen;
		[Inject] private IUiUpgradesScreen _uiUpgradesScreen;


		readonly private CompositeDisposable _disposable = new CompositeDisposable();
		private IDisposable _summonInterruptDisposable;

		public void Initialize()
		{
			bool isComplete = _profile.Tutorial.UpgradesStep.Value == UpgradesStep.Complete;
			int startLevel = _menuConfig.GetAccessLevel(GameState.Upgrades);

			if (isComplete)
				return;

			SetProfileStepValue(UpgradesStep.None);
			//Transition(UpgradesStep.None);

			_profile.Tutorial.UpgradesStep
				.Where(step => step != State)
				.Subscribe(OnUpgradesStepChanged)
				.AddTo(_disposable);

			_gameCycle.State
				.Where(gameState =>
					gameState == GameState.Lobby &&
					State == UpgradesStep.None &&
					isComplete == false &&
					_profile.LevelNumber.Value >= startLevel
				)
				.Subscribe(_ => SetProfileStepValue(UpgradesStep.MenuButton))
				.AddTo(_disposable);

			UiMainMenuButton upgradeButton = _uiMainMenuView.GetButton(GameState.Upgrades);
			upgradeButton.ButtonClicked
				.Where(_ => State == UpgradesStep.MenuButton)
				.Subscribe(_ =>
				{
					int infantryLevel = _gameUpgrades.GetUnitLevel(Species.HeroInfantryman);
					int sniperLevel = _gameUpgrades.GetUnitLevel(Species.HeroSniper);
					UpgradesStep nextStep = (infantryLevel == 1) 
						? UpgradesStep.FirstUpgrade 
						: (sniperLevel == 1) ? UpgradesStep.SelectNextUnit: UpgradesStep.UpgradeHint;
					SetProfileStepValue(nextStep);
				})
				.AddTo(_disposable);

			_gameUpgrades.Upgraded
				.Where(_ => State == UpgradesStep.FirstUpgrade)
				.Subscribe(_ => SetProfileStepValue(UpgradesStep.SelectNextUnit))
				.AddTo(_disposable);

			_uiUpgradesScreen.UnitElements[Species.HeroSniper].SelectButtonClicked
				.Where(_ => State == UpgradesStep.SelectNextUnit)
				.Subscribe(_ => SetProfileStepValue(UpgradesStep.SecondUpgrade))
				.AddTo(_disposable);

			_gameUpgrades.Upgraded
				.Where(_ => State == UpgradesStep.SecondUpgrade)
				.Subscribe(_ => SetProfileStepValue(UpgradesStep.UpgradeHint))
				.AddTo(_disposable);

			_dialogHint.NextMessageButtonClicked
				.Where(_ => State == UpgradesStep.UpgradeHint)
				.Subscribe(_ => SetProfileStepValue(UpgradesStep.GoToLobby))
				.AddTo(_disposable);

			_gameCycle.State
				.Where(state =>
					state == GameState.Lobby &&
					State == UpgradesStep.GoToLobby
				)
				.Subscribe(_ => SetProfileStepValue(UpgradesStep.GoToBattle))
				.AddTo(_disposable);

			_gameLevel.LevelLoading
				.Where(_ => State == UpgradesStep.GoToBattle)
				.Subscribe(_ => SetProfileStepValue(UpgradesStep.Complete))
				.AddTo(_disposable);
		}

		public virtual void Dispose()
		{
			_summonInterruptDisposable?.Dispose();
			_disposable.Dispose();
		}

		protected override void StateTransitions()
		{
			switch (State)
			{
				case UpgradesStep.MenuButton:
					break;

			}
		}

		#region MenuButton Step

		protected override void OnEnterMenuButton()
		{
			_fingerHint.Show(FingerPlace.MainMenuUpgrade);
			ActivateDialogMessege();
			_uiLobbyScreen.SetPlayButtonEnabled(false);
		}
		#endregion

		#region MenuButton Step

		protected override void OnEnterFirstUpgrade()
		{
			_uiMainMenuView.SetButtonInteractable(GameState.Lobby, false);
			_uiUpgradesScreen.UnitElements[Species.HeroSniper].SetSelectInteractable(false);
			_uiUpgradesScreen.UnitElements[Species.HeroSniper].SetUpgradeInteractable(false);
			_uiUpgradesScreen.UnitElements[Species.HeroInfantryman].SetSelectInteractable(false);

			HintedButton hintedButton = _uiUpgradesScreen.UnitElements[Species.HeroInfantryman].UpgradeButton.GetComponent<HintedButton>();
			ShowUpgradeFingerHint(hintedButton.HintParameters);

			ActivateDialogMessege();
		}
		#endregion

		#region SelectNextUnit Step

		protected override void OnEnterSelectNextUnit()
		{
			_uiUpgradesScreen.UnitElements[Species.HeroSniper].SetSelectInteractable(true);
			_uiUpgradesScreen.UnitElements[Species.HeroInfantryman].SetUpgradeInteractable(false);

			HintedButton hintedButton = _uiUpgradesScreen.UnitElements[Species.HeroSniper].SelectButton.GetComponent<HintedButton>();
			ShowUpgradeFingerHint(hintedButton.HintParameters);

			ActivateDialogMessege();
		}
		#endregion

		#region SecondUpgrade Step

		protected override void OnEnterSecondUpgrade()
		{
			_uiUpgradesScreen.UnitElements[Species.HeroSniper].SetSelectInteractable(false);
			_uiUpgradesScreen.UnitElements[Species.HeroSniper].SetUpgradeInteractable(true);

			HintedButton hintedButton = _uiUpgradesScreen.UnitElements[Species.HeroSniper].UpgradeButton.GetComponent<HintedButton>();
			ShowUpgradeFingerHint(hintedButton.HintParameters);

			ActivateDialogMessege();
		}
		#endregion

		#region UpgradeHint Step

		protected override void OnEnterUpgradeHint()
		{
			_uiUpgradesScreen.UnitElements[Species.HeroInfantryman].SetSelectInteractable(false);
			_uiUpgradesScreen.UnitElements[Species.HeroInfantryman].SetUpgradeInteractable(false);
			_uiUpgradesScreen.UnitElements[Species.HeroSniper].SetSelectInteractable(false);
			_uiUpgradesScreen.UnitElements[Species.HeroSniper].SetUpgradeInteractable(false);
			_fingerHint.Hide();
			ActivateDialogMessege();
			_dialogHint.SetNextMessageIndicatorActive(true);
			_dialogHint.SetNextMessageButtonActive(true);
		}

		protected override void OnExitUpgradeHint()
		{
			_dialogHint.SetNextMessageIndicatorActive(false);
			_dialogHint.SetNextMessageButtonActive(false);
			_dialogHint.SetActive(false);
		}

		#endregion

		#region GoToLobby Step

		protected override void OnEnterGoToLobby()
		{
			_uiMainMenuView.SetButtonInteractable(GameState.Lobby, true);
			_fingerHint.Show(FingerPlace.MainMenuHome);
			ActivateDialogMessege();
		}

		#endregion

		#region GoToBattle Step

		protected override void OnEnterGoToBattle()
		{
			_uiLobbyScreen.SetPlayButtonEnabled(true);
			_fingerHint.Show(FingerPlace.LobbyBattle);
			_uiMainMenuView.SetButtonInteractable(GameState.Upgrades, false);
			ActivateDialogMessege();
		}

		protected override void OnExitGoToBattle()
		{
			_uiUpgradesScreen.UnitElements[Species.HeroInfantryman].SetSelectInteractable(true);
			_uiUpgradesScreen.UnitElements[Species.HeroInfantryman].SetUpgradeInteractable(true);
			_uiUpgradesScreen.UnitElements[Species.HeroSniper].SetSelectInteractable(true);
			_uiUpgradesScreen.UnitElements[Species.HeroSniper].SetUpgradeInteractable(true);
			_fingerHint.Hide();
			_dialogHint.SetActive(false);
		}

		#endregion

		private void ShowUpgradeFingerHint(HintedButton.Parameters hint)
		{
			_fingerHint.SetPosition(hint.Point);
			_fingerHint.SetLeft(hint.IsLeft);
			_fingerHint.Show(FingerPlace.None);
		}

		private void SetProfileStepValue(UpgradesStep step) =>
			_profile.Tutorial.UpgradesStep.Value = step;

		private void OnUpgradesStepChanged(UpgradesStep step)
        {
			_gameProfileManager.Save();
			Transition(step);

            if (step == UpgradesStep.Complete)
                Dispose();
        }

        private void ActivateDialogMessege()
        {
            if (_config.UpgradesTutorialMessages.TryGetValue(State, out TutorialMessage<UpgradesStep> data))
            {
                _dialogHint.SetMessage(_localizator.GetString(data.TranslationKey));
				_dialogHint.SetPlace(data.Place);
				_dialogHint.SetActive(true);
            }
        }
    }
}