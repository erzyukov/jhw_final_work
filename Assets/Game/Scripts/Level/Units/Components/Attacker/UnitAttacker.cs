namespace Game.Units
{
	using Game.Configs;
	using Game.Core;
	using Zenject;

	public class UnitAttacker : UnitAttackerBase, IInitializable, IUnitAttacker
	{
		[Inject] IBattleEvents _battleEvents;
		[Inject] UnitConfig _unitConfig;

		#region IUnitAttacker

		// TODO: refact: all damage through DamageApplyed battle event
        public override void Attack(IUnitFacade target)
        {
			base.Attack(target);

			if (target == null)
				return;

			if (_unitConfig.AoeRange == 0)
			{
				target.TakeDamage(CurrentDamage);
			}
			else
			{
				_battleEvents.DamageApplyed.Execute( new() {
					Target			= target,
					Amount			= CurrentDamage,
					Type			= EDamageType.Aoe,
					VfxType			= _unitConfig.DamageVfx,
					Range			= _unitConfig.AoeRange,
					Position		= target.Transform.position,
					ProjectileType	= Weapon.ProjectileType.None
				} );
			}

            AtackTimer.Set(_unitConfig.AttackDelay);
        }

		#endregion
	}
}