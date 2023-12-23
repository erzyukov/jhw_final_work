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
	using System.Collections.Generic;

	public class UpgradesTutorial : UpgradesTutorialFsmBase, IInitializable, IDisposable
    {
        [Inject] private GameProfile _profile;
        [Inject] private IGameProfileManager _gameProfileManager;
        [Inject] private IGameCycle _cycle;
        [Inject] private ILocalizator _localizator;
        [Inject] private MenuConfig _menuConfig;
        [Inject] private TutorialConfig _config;
        [Inject] private IFingerHint _fingerHint;
		[Inject] private IDialogHint _dialogHint;
        //[Inject] private IUiMainMenuView _uiMainMenuView;

        readonly private CompositeDisposable _disposable = new CompositeDisposable();
        private IDisposable _summonInterruptDisposable;

        public void Initialize()
        {
            bool isComplete = _profile.Tutorial.UpgradesStep.Value == UpgradesStep.Complete;
            int startLevel = _menuConfig.GetAccessLevel(GameState.Upgrades);

            _cycle.State
                .Where(state =>
                    state == GameState.LoadingLobby &&
                    isComplete == false &&
                    _profile.LevelNumber.Value >= startLevel
                )
                .Subscribe(_ => _profile.Tutorial.UpgradesStep.Value = UpgradesStep.MenuButton)
                .AddTo(_disposable);

            _profile.Tutorial.UpgradesStep
                .Where(step => step != State)
                .Subscribe(OnUpgradesStepChanged)
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
        }

        protected override void OnExitMenuButton()
        {
            _fingerHint.Hide();
            _dialogHint.SetActive(false);
        }

        #endregion
    
        private void OnUpgradesStepChanged(UpgradesStep step)
        {
            _gameProfileManager.Save();
            Transition(step);

            if (step == UpgradesStep.Complete)
                Dispose();
        }

        private void ActivateDialogMessege()
        {
            if (_config.UpgradesTutorialMessages.TryGetValue(State, out string key))
            {
                _dialogHint.SetMessage(_localizator.GetString(key));
                _dialogHint.SetActive(true);
            }
        }
    }
}