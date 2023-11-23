namespace Game.Units
{
	public enum UnitState
	{
		None,

		Idle,
		Draging,
		Merging,
		SearchTarget,
		TargetFound,
		MoveToTarget,
		Attack,
		TargetLost,
		Died,
	}
}
