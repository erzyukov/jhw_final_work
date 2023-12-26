namespace Game.Tutorial
{
    using Game.Utilities;
    using Zenject;

    public abstract class UpgradesTutorialFsmBase : SimpleFsmBase<UpgradesStep>, ITickable
    {
        protected UpgradesTutorialFsmBase()
        {
            AddOnEnterAction(UpgradesStep.MenuButton, OnEnterMenuButton);
            //AddOnExitAction(UpgradesStep.MenuButton, OnExitMenuButton);
            AddOnEnterAction(UpgradesStep.FirstUpgrade, OnEnterFirstUpgrade);
            //AddOnExitAction(UpgradesStep.FirstUpgrade, OnExitFirstUpgrade);
            AddOnEnterAction(UpgradesStep.SelectNextUnit, OnEnterSelectNextUnit);
            //AddOnExitAction(UpgradesStep.SelectNextUnit, OnExitSelectNextUnit);
            AddOnEnterAction(UpgradesStep.SecondUpgrade, OnEnterSecondUpgrade);
            //AddOnExitAction(UpgradesStep.SecondUpgrade, OnExitSecondUpgrade);
            AddOnEnterAction(UpgradesStep.UpgradeHint, OnEnterUpgradeHint);
			AddOnExitAction(UpgradesStep.UpgradeHint, OnExitUpgradeHint);
			AddOnEnterAction(UpgradesStep.GoToLobby, OnEnterGoToLobby);
			AddOnEnterAction(UpgradesStep.GoToBattle, OnEnterGoToBattle);
            AddOnExitAction(UpgradesStep.GoToBattle, OnExitGoToBattle);
        }

        protected override UpgradesStep DefaultState => UpgradesStep.None;

        protected abstract void OnEnterMenuButton();
        //protected abstract void OnExitMenuButton();
        protected abstract void OnEnterFirstUpgrade();
        //protected abstract void OnExitFirstUpgrade();
        protected abstract void OnEnterSelectNextUnit();
        //protected abstract void OnExitSelectNextUnit();
        protected abstract void OnEnterSecondUpgrade();
        //protected abstract void OnExitSecondUpgrade();
        protected abstract void OnEnterUpgradeHint();
        protected abstract void OnExitUpgradeHint();
        protected abstract void OnEnterGoToLobby();
        protected abstract void OnEnterGoToBattle();
        protected abstract void OnExitGoToBattle();
    }
}