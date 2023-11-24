namespace Game.Units
{
	using Game.Utilities;
	using Zenject;

    public abstract class UnitFsmBase : SimpleFsmBase<UnitState>, ITickable
	{
		protected UnitFsmBase()
		{
			AddOnEnterAction(UnitState.Idle, OnEnterIdleHandler);
			AddOnEnterAction(UnitState.SearchTarget, OnEnterSearchTargetHandler);
			AddOnEnterAction(UnitState.TargetLost, OnEnterTargetLostHandler);
			AddOnEnterAction(UnitState.Died, OnEnterDiedHandler);
		}

		protected override UnitState DefaultState => UnitState.None;

		protected abstract void OnEnterIdleHandler();
		protected abstract void OnEnterSearchTargetHandler();
		protected abstract void OnEnterTargetLostHandler();
		protected abstract void OnEnterDiedHandler();
	}
}