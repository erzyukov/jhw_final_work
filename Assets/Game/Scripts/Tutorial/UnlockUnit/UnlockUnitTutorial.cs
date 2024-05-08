namespace Game.Tutorial
{
	using Game.Configs;
	using Game.Core;
	using Game.Profiles;
	using Game.Ui;
	using Game.Units;
	using Sirenix.Utilities;
	using System;
	using System.Linq;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public class UnlockUnitTutorial : UnlockUnitTutorialFsmBase, IInitializable, IDisposable
	{
		[Inject] private GameProfile			_profile;
		[Inject] private IGameProfileManager	_gameProfileManager;
		[Inject] private IGameCycle				_gameCycle;
		[Inject] private UpgradesConfig			_upgradesConfig;
		[Inject] private IUiMainMenuView		_uiMainMenuView;
		[Inject] private UnitsConfig			_unitsConfig;
		[Inject] private IUiLobbyFlow			_uiLobbyFlow;
		[Inject] private IUiUpgradeFlow			_uiUpgradeFlow;
		[Inject] private IFingerHint			_fingerHint;
		[Inject] private TutorialConfig			_config;
		[Inject] private ILocalizator			_localizator;
		[Inject] private IDialogHint			_dialogHint;
		[Inject] private IGameUpgrades			_gameUpgrades;

		readonly private CompositeDisposable	_disposable = new CompositeDisposable();

		private const Species		UnlockSpecies	= Species.GrenadeLauncher;

		private bool IsComplete		=> _profile.Tutorial.UnlockUnitStep.Value == UnlockUnitStep.Complete;

		private int _levelForUnlock;

		public void Initialize()
		{
			_gameProfileManager.Save();

			if (IsComplete)
				return;

			_uiLobbyFlow.Loaded
				.Subscribe( _ =>
				{
					InitTutorial();
					StepSubscribes();
				} )
				.AddTo( _disposable );
		}

		public void Dispose()		=> _disposable.Dispose();

		private void InitTutorial()
		{
			_levelForUnlock		= _upgradesConfig.UnitsUpgrades[UnlockSpecies].UnlockHeroLevel;

			_profile.Tutorial.UnlockUnitStep
				.Where(step => step != State)
				.Subscribe(OnStepChanged)
				.AddTo(_disposable);
		}

		private void StepSubscribes()
		{
			// Start Tutorial => MenuButton
			_gameCycle.State
				.Where(gameState =>
					gameState	== GameState.Lobby		&&
					IsComplete	== false				&&
					_profile.HeroLevel.Value >= _levelForUnlock
				)
				.Subscribe(_ => SetProfileStepValue(UnlockUnitStep.MenuButton))
				.AddTo(_disposable);
			
			// => UnlockUnit
			UiMainMenuButton upgradeButton = _uiMainMenuView.GetButton(GameState.Upgrades);
			upgradeButton.ButtonClicked
				.Where(_ => State == UnlockUnitStep.MenuButton)
				.Subscribe(_ =>
				{
					SetActiveAllSelection( false );
					SetActiveAllUpgrades( false );

					int grenadierLevel		= _gameUpgrades.GetUnitLevel( Species.GrenadeLauncher );
					UnlockUnitStep nextStep = (grenadierLevel == 0)
						? UnlockUnitStep.UnlockUnit
						: UnlockUnitStep.UnlockHint;

					Debug.LogWarning($">> ButtonClicked >> {nextStep}");

					SetProfileStepValue(nextStep);
				})
				.AddTo(_disposable);

			// => UnlockHint
			_gameUpgrades.Upgraded
				.Where(_ => State == UnlockUnitStep.UnlockUnit)
				.Subscribe(_ => SetProfileStepValue(UnlockUnitStep.UnlockHint))
				.AddTo(_disposable);

			// => Complete
			_dialogHint.NextMessageButtonClicked
				.Where(_ => State == UnlockUnitStep.UnlockHint)
				.Subscribe(_ => SetProfileStepValue(UnlockUnitStep.Complete))
				.AddTo(_disposable);
		}

		private void SetProfileStepValue(UnlockUnitStep step) =>
			_profile.Tutorial.UnlockUnitStep.Value = step;

		private void OnStepChanged(UnlockUnitStep step)
        {
			_gameProfileManager.Save();
			Transition( step );

            if (step == UnlockUnitStep.Complete)
                Dispose();
        }

		#region MenuButton Step

		protected override void OnEnterMenuButton()
		{
			_fingerHint.Show( FingerPlace.MainMenuUpgrade );
			ActivateDialogMessege();
			_uiLobbyFlow.IsSelectLevelAvailable.Value	= false;
			_uiLobbyFlow.IsStartAvailable.Value			= false;
			SetMenuButtonsActive( false );
			_uiMainMenuView.SetButtonInteractable(GameState.Upgrades, true);
		}

		#endregion

		#region UnlockUnit Step

		protected override void OnEnterUnlockUnit()
		{
			SetMenuButtonsActive( false );
			_uiMainMenuView.SetButtonInteractable(GameState.Upgrades, true);
			_uiUpgradeFlow.SelectedUnit.Value = Species.GrenadeLauncher;

			SetActiveUpgrade( Species.GrenadeLauncher, true );
			_dialogHint.SetActive(false);

			HintedButton hintedButton = _uiUpgradeFlow.UpgradeButtons[Species.GrenadeLauncher].GetComponent<HintedButton>();
			ShowUpgradeFingerHint( hintedButton.HintParameters );
		}

		#endregion

		#region UnlockHint Step

		protected override void OnEnterUnlockHint()
		{
			_fingerHint.Hide();
			SetActiveUpgrade( Species.GrenadeLauncher, false );
			ActivateDialogMessege();
			_dialogHint.SetNextMessageIndicatorActive(true);
			_dialogHint.SetNextMessageButtonActive(true);
		}

		protected override void OnExitUnlockHint()
		{
			_uiLobbyFlow.IsSelectLevelAvailable.Value	= true;
			_uiLobbyFlow.IsStartAvailable.Value			= true;
			_dialogHint.SetNextMessageIndicatorActive(false);
			_dialogHint.SetNextMessageButtonActive(false);
			_dialogHint.SetActive(false);
			SetActiveAllSelection( true );
			SetActiveAllUpgrades( true );
			SetMenuButtonsActive( true );
		}

		#endregion


        private void ActivateDialogMessege()
        {
            if (_config.UnlockStepTutorialMessages.TryGetValue(State, out var data))
            {
				_dialogHint.SetMessage( _localizator.GetString( data.TranslationKey ) );
				_dialogHint.SetPlace( data.Place );
				_dialogHint.SetActive( true );
            }
        }

		private void SetMenuButtonsActive( bool value )
		{
			Enum.GetValues( typeof( GameState ) )
				.Cast<GameState>()
				.ForEach( v => _uiMainMenuView.SetButtonInteractable( v, value ) );
		}

		// TODO: Need refact code duplicate [UpgradesTutorial]
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

		private void ShowUpgradeFingerHint(HintedButton.Parameters hint)
		{
			_fingerHint.SetPosition( hint.Point );
			_fingerHint.SetLeft( hint.IsLeft );
			_fingerHint.Show( FingerPlace.None );
		}
	}
}
