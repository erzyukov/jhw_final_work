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

	public class UpgradesTutorial : UpgradesTutorialFsmBase, IInitializable, IDisposable
	{
		[Inject] private GameProfile			_profile;
		[Inject] private IGameProfileManager	_gameProfileManager;
		[Inject] private IGameCycle				_gameCycle;
		[Inject] private IGameUpgrades			_gameUpgrades;
		[Inject] private ILocalizator			_localizator;
		[Inject] private MenuConfig				_menuConfig;
		[Inject] private TutorialConfig			_config;
		[Inject] private UnitsConfig			_unitsConfig;
		[Inject] private IFingerHint			_fingerHint;
		[Inject] private IDialogHint			_dialogHint;
		[Inject] private IUiMainMenuView		_uiMainMenuView;
		[Inject] private IUiLobbyFlow			_uiLobbyFlow;
		[Inject] private IScenesManager			_scenesManager;
		[Inject] private IUiUpgradeFlow			_uiUpgradeFlow;

		readonly private CompositeDisposable _disposable = new CompositeDisposable();

		public void Initialize()
		{
			if (_profile.Tutorial.UpgradesStep.Value == UpgradesStep.Complete)
				return;

			InitTutorial();
			StepSubscribes();
		}

		private void InitTutorial()
		{
			if (_profile.Tutorial.UpgradesStep.Value >= UpgradesStep.UpgradeHint)
				SetProfileStepValue(UpgradesStep.GoToBattle);
			else if (_profile.Tutorial.UpgradesStep.Value != UpgradesStep.None)
				SetProfileStepValue(UpgradesStep.None);

			_profile.Tutorial.UpgradesStep
				.Where(step => step != State)
				.Subscribe(OnUpgradesStepChanged)
				.AddTo(_disposable);

			_scenesManager.MainLoaded
				.Where(_ => _profile.Tutorial.UpgradesStep.Value == UpgradesStep.GoToBattle)
				.Subscribe(_ => _uiMainMenuView.SetButtonInteractable(GameState.Upgrades, false))
				.AddTo(_disposable);
		}

		private void StepSubscribes()
		{
			bool isComplete = _profile.Tutorial.UpgradesStep.Value == UpgradesStep.Complete;
			int startLevel = _menuConfig.GetAccessLevel(GameState.Upgrades);

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
					SetActiveAllSelection( false );
					SetActiveAllUpgrades( false );
					SetActiveUpgrade( Species.HeroInfantryman, true );

					int infantryLevel = _gameUpgrades.GetUnitLevel(Species.HeroInfantryman);
					int sniperLevel = _gameUpgrades.GetUnitLevel(Species.HeroSniper);
					UpgradesStep nextStep = (infantryLevel == 1)
						? UpgradesStep.FirstUpgrade
						: (sniperLevel == 1) ? UpgradesStep.SelectNextUnit : UpgradesStep.UpgradeHint;
					SetProfileStepValue(nextStep);
				})
				.AddTo(_disposable);

			_gameUpgrades.Upgraded
				.Where(_ => State == UpgradesStep.FirstUpgrade)
				.Subscribe(_ => SetProfileStepValue(UpgradesStep.SelectNextUnit))
				.AddTo(_disposable);

			_uiUpgradeFlow.SelectedUnit
				.Where( s => s == Species.HeroSniper && State == UpgradesStep.SelectNextUnit )
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

			_gameCycle.State
				.Where(state =>
					state == GameState.LoadingLevel &&
					State == UpgradesStep.GoToBattle
				)
				.Subscribe(_ => SetProfileStepValue(UpgradesStep.Complete))
				.AddTo(_disposable);
		}

		public virtual void Dispose() =>
			_disposable.Dispose();

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
			_uiLobbyFlow.IsSelectLevelAvailable.Value = false;
			_uiLobbyFlow.IsStartAvailable.Value = false;
		}
		#endregion

		#region MenuButton Step

		protected override void OnEnterFirstUpgrade()
		{
			_uiMainMenuView.SetButtonInteractable(GameState.Lobby, false);
			SetActiveSelection( Species.HeroInfantryman, false );
			SetActiveSelection( Species.HeroSniper, false );
			SetActiveUpgrade( Species.HeroSniper, false );

			HintedButton hintedButton = _uiUpgradeFlow.UpgradeButtons[Species.HeroInfantryman].GetComponent<HintedButton>();
			ShowUpgradeFingerHint(hintedButton.HintParameters);

			ActivateDialogMessege();
		}
		#endregion

		#region SelectNextUnit Step

		protected override void OnEnterSelectNextUnit()
		{
			_uiMainMenuView.SetButtonInteractable(GameState.Lobby, false);

			SetActiveSelection( Species.HeroSniper, true );
			SetActiveUpgrade( Species.HeroInfantryman, false );

			HintedButton hintedButton = _uiUpgradeFlow.SelectButtons[Species.HeroSniper].GetComponent<HintedButton>();
			ShowUpgradeFingerHint(hintedButton.HintParameters);

			ActivateDialogMessege();
		}
		#endregion

		#region SecondUpgrade Step

		protected override void OnEnterSecondUpgrade()
		{
			SetActiveSelection( Species.HeroSniper, false );
			SetActiveUpgrade( Species.HeroSniper, true );

			HintedButton hintedButton = _uiUpgradeFlow.UpgradeButtons[Species.HeroSniper].GetComponent<HintedButton>();
			ShowUpgradeFingerHint(hintedButton.HintParameters);

			ActivateDialogMessege();
		}
		#endregion

		#region UpgradeHint Step

		protected override void OnEnterUpgradeHint()
		{
			SetActiveAllSelection( false );
			SetActiveAllUpgrades( false );

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
			_uiLobbyFlow.IsStartAvailable.Value = true;
			_fingerHint.Show(FingerPlace.LobbyBattle);
			_uiMainMenuView.SetButtonInteractable(GameState.Upgrades, false);
			ActivateDialogMessege();
		}

		protected override void OnExitGoToBattle()
		{
			_uiLobbyFlow.IsSelectLevelAvailable.Value = true;
			SetActiveAllSelection( true );
			SetActiveAllUpgrades( true );

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
            if (_config.UpgradesTutorialMessages.TryGetValue(State, out TutorialConfig.TutorialMessage<UpgradesStep> data))
            {
                _dialogHint.SetMessage(_localizator.GetString(data.TranslationKey));
				_dialogHint.SetPlace(data.Place);
				_dialogHint.SetActive(true);
            }
        }

		private void SetActiveAllSelection( bool value ) =>
			_unitsConfig.HeroUnits.ForEach( s => SetActiveSelection( s, value ) );

		private void SetActiveAllUpgrades( bool value ) =>
			_unitsConfig.HeroUnits.ForEach( s => SetActiveUpgrade( s, value ) );

		private void SetActiveSelection( Species species, bool value )
		{
			if (value)
				_uiUpgradeFlow.SelectDisabled.Remove( species );
			else
				_uiUpgradeFlow.SelectDisabled.Add( species );
		}

		private void SetActiveUpgrade( Species species, bool value )
		{
			if (value)
				_uiUpgradeFlow.UpgradeDisabled.Remove( species );
			else
				_uiUpgradeFlow.UpgradeDisabled.Add( species );
		}
    }
}