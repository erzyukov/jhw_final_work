namespace Game.Tutorial
{
    using Game.Utilities;
    using Zenject;

    public abstract class UpgradesTutorialFsmBase : SimpleFsmBase<UpgradesStep>, ITickable
    {
        protected UpgradesTutorialFsmBase()
        {
            AddOnEnterAction(UpgradesStep.MenuButton, OnEnterMenuButton);
            AddOnExitAction(UpgradesStep.MenuButton, OnExitMenuButton);
        }

        protected override UpgradesStep DefaultState => UpgradesStep.None;

        protected abstract void OnEnterMenuButton();
        protected abstract void OnExitMenuButton();
    }
}