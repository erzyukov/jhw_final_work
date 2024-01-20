namespace Game.Tutorial
{
    using Game.Utilities;
    using Zenject;

    public abstract class UpgradesTutorialFsmBase : SimpleFsmBase<UpgradesStep>, ITickable
    {
        protected UpgradesTutorialFsmBase()
        {
            AddOnEnterAction(UpgradesStep.MenuButton, OnEnterMenuButton);
            AddOnEnterAction(UpgradesStep.FirstUpgrade, OnEnterFirstUpgrade);
            AddOnEnterAction(UpgradesStep.SelectNextUnit, OnEnterSelectNextUnit);
            AddOnEnterAction(UpgradesStep.SecondUpgrade, OnEnterSecondUpgrade);
            AddOnEnterAction(UpgradesStep.UpgradeHint, OnEnterUpgradeHint);
			AddOnExitAction(UpgradesStep.UpgradeHint, OnExitUpgradeHint);
			AddOnEnterAction(UpgradesStep.GoToLobby, OnEnterGoToLobby);
			AddOnEnterAction(UpgradesStep.GoToBattle, OnEnterGoToBattle);
            AddOnExitAction(UpgradesStep.GoToBattle, OnExitGoToBattle);
        }

        protected override UpgradesStep DefaultState => UpgradesStep.None;

        protected abstract void OnEnterMenuButton();
        protected abstract void OnEnterFirstUpgrade();
        protected abstract void OnEnterSelectNextUnit();
        protected abstract void OnEnterSecondUpgrade();
        protected abstract void OnEnterUpgradeHint();
        protected abstract void OnExitUpgradeHint();
        protected abstract void OnEnterGoToLobby();
        protected abstract void OnEnterGoToBattle();
        protected abstract void OnExitGoToBattle();
    }
}