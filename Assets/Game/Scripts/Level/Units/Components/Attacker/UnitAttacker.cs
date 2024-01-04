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
			target.TakeDamage(CurrentDamage);
            AtackTimer.Set(_unitConfig.AttackDelay);
        }

		#endregion
	}
}