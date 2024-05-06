namespace Game.Tutorial
{
    using Game.Utilities;
    using Zenject;

    public abstract class UnlockUnitTutorialFsmBase : SimpleFsmBase<UnlockUnitStep>, ITickable
    {
        protected UnlockUnitTutorialFsmBase()
        {
            AddOnEnterAction(UnlockUnitStep.MenuButton, OnEnterMenuButton);
            AddOnEnterAction(UnlockUnitStep.UnlockUnit, OnEnterUnlockUnit);
            AddOnEnterAction(UnlockUnitStep.UnlockHint, OnEnterUnlockHint);
			AddOnExitAction(UnlockUnitStep.UnlockHint, OnExitUnlockHint);
        }

        protected override UnlockUnitStep DefaultState => UnlockUnitStep.None;

        protected abstract void OnEnterMenuButton();
        protected abstract void OnEnterUnlockUnit();
        protected abstract void OnEnterUnlockHint();
        protected abstract void OnExitUnlockHint();
    }
}