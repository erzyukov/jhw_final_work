namespace Game.Units
{
	using Game.Configs;
	using Zenject;

	public class UnitAttacker : UnitAttackerBase, IInitializable, IUnitAttacker
	{
		[Inject] UnitConfig _unitConfig;

		#region IUnitAttacker

        public override void Attack(IUnitFacade target)
        {
			base.Attack(target);

			if (target == null)
				return;

			target.TakeDamage(CurrentDamage);
            AtackTimer.Set(_unitConfig.AttackDelay);
        }

		#endregion
	}
}