namespace Game.Units
{
	using Game.Utilities;
	using Zenject;

    public abstract class UnitFsmBase : SimpleFsmBase<UnitState>, ITickable
	{
		protected UnitFsmBase()
		{
			AddOnEnterAction(UnitState.Idle, OnEnteredIdle);
			AddOnEnterAction(UnitState.SearchTarget, OnEnteredSearchTarget);
			AddOnEnterAction(UnitState.MoveToTarget, OnEnteredMoveToTarget);
			AddOnExitAction(UnitState.MoveToTarget, OnExitedMoveToTarget);
			AddOnEnterAction(UnitState.PrepareAttack, OnEnteredPrepareAttack);
			AddOnEnterAction(UnitState.StartAttack, OnEnteredStartAttack);
			AddOnEnterAction(UnitState.TargetLost, OnEnteredTargetLost);
			AddOnEnterAction(UnitState.Dying, OnEnteredDying);
			AddOnEnterAction(UnitState.Dead, OnEnteredDead);
		}

		protected override UnitState DefaultState => UnitState.None;

		protected abstract void OnEnteredIdle();
		protected abstract void OnEnteredSearchTarget();
		protected abstract void OnEnteredMoveToTarget();
		protected abstract void OnExitedMoveToTarget();
		protected abstract void OnEnteredPrepareAttack();
		protected abstract void OnEnteredStartAttack();
		protected abstract void OnEnteredTargetLost();
		protected abstract void OnEnteredDying();
		protected abstract void OnEnteredDead();
	}
}