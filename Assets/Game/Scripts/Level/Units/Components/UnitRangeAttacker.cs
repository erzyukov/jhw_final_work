namespace Game.Units
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Configs;
	using UnityEngine;
	using Game.Weapon;
	using System;
	using System.Collections.Generic;

	public class UnitRangeAttacker : ControllerBase, IInitializable, IUnitAttacker
	{
		[Inject] private IUnitView _view;
		[Inject] private UnitGrade _grade;
		[Inject] private UnitConfig _config;
		[Inject] private IProjectileSpawner _projectileSpawner;

		private ITimer _atackTimer;
		private Transform _shootPoint;
		private float _damage;

		public void Initialize()
		{
			ShootPoint shootPointComponent = _view.Transform.GetComponentInChildren<ShootPoint>();
			
			if (shootPointComponent != null )
				_shootPoint = _view.Transform.GetComponentInChildren<ShootPoint>().transform;
			else
				throw new Exception("Can't found component ShootPoint in UnitView object!");

			// TODO: Calculate damage
			_damage = _grade.Damage;
			_atackTimer = new Timer();
		}

		#region IUnitAttacker

		public ReactiveCommand AttackRangeBroken { get; } = new ReactiveCommand();

		public void TryAttack(IUnitFacade target)
		{
			if (IsTargetClose(target) == false)
			{
				AttackRangeBroken.Execute();
				return;
			}

			if (_atackTimer.IsReady == false)
				return;

			_projectileSpawner.SpawnBullet(_shootPoint.transform.position, target, _damage);
			_atackTimer.Set(_grade.AttackDelay);
		}

		#endregion

		private bool IsTargetClose(IUnitFacade target) =>
			(_view.Transform.position - target.Transform.position).sqrMagnitude
			< _config.AttackRange * _config.AttackRange + Mathf.Epsilon;
	}
}