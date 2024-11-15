﻿namespace Game.Units
{
	using Zenject;
	using UnityEngine;
	using Game.Weapon;
	using System;
	using Game.Configs;

	public class UnitRangeAttacker : UnitAttackerBase, IInitializable, IUnitAttacker
	{
		[Inject] private IUnitView _view;
		[Inject] UnitConfig _unitConfig;
		[Inject] private IProjectileSpawner _projectileSpawner;

		private Transform _shootPoint;

		public override void Initialize()
		{
			base.Initialize();
			
			ShootPoint shootPointComponent = _view.Transform.GetComponentInChildren<ShootPoint>();
			
			if (shootPointComponent != null )
				_shootPoint = _view.Transform.GetComponentInChildren<ShootPoint>().transform;
			else
				throw new Exception("Can't found component ShootPoint in UnitView object!");
		}

		#region IUnitAttacker

		public override void Attack(IUnitFacade target)
        {
			base.Attack(target);

			if (target == null)
				return;

			_projectileSpawner.Spawn(
				_shootPoint.transform.position, 
				_unitConfig.ProjectileType, 
				target, 
				CurrentDamage,
				_unitConfig.AttackRange
			);
			AtackTimer.Set(_unitConfig.AttackDelay);
        }

        #endregion
    }
}