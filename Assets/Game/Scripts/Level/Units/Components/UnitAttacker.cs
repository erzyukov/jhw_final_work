namespace Game.Units
{
	using Zenject;

	public class UnitAttacker : UnitAttackerBase, IInitializable, IUnitAttacker
	{
		[Inject] UnitGrade _grade;

		#region IUnitAttacker

        public override void Attack(IUnitFacade target)
        {
			target.TakeDamage(CurrentDamage);
            AtackTimer.Set(_grade.AttackDelay);
        }

		#endregion
	}
}