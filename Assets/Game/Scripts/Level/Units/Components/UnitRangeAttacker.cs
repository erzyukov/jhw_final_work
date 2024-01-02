namespace Game.Units
{
	using Game.Utilities;
	using Zenject;
	using Game.Configs;
	using UnityEngine;
	using Game.Weapon;
	using System;
	using Game.Core;

	public class UnitRangeAttacker : ControllerBase, IInitializable, IUnitAttacker
	{
		[Inject] private IUnitView _view;
		[Inject] private Species _species;
		[Inject] private IGameUpgrades _gameUpgrades;
		[Inject] private UnitGrade _grade;
		[Inject] private UnitConfig _config;
		[Inject] private IProjectileSpawner _projectileSpawner;

		private ITimer _atackTimer;
		private Transform _shootPoint;
		private float _currentDamage;

		public void Initialize()
		{
			ShootPoint shootPointComponent = _view.Transform.GetComponentInChildren<ShootPoint>();
			
			if (shootPointComponent != null )
				_shootPoint = _view.Transform.GetComponentInChildren<ShootPoint>().transform;
			else
				throw new Exception("Can't found component ShootPoint in UnitView object!");

			_currentDamage = _gameUpgrades.GetUnitDamage(_species) * _grade.DamageMultiplier;
			_atackTimer = new Timer();
		}

		#region IUnitAttacker

		public bool IsReadyToAttack => _atackTimer.IsReady;

		public void Attack(IUnitFacade target)
        {
			_projectileSpawner.SpawnBullet(_shootPoint.transform.position, target, _currentDamage);
			_atackTimer.Set(_grade.AttackDelay);
        }

        public bool IsTargetClose(IUnitFacade target) =>
            target != null &&
            target.IsDead != true &&
            (_view.Transform.position - target.Transform.position).sqrMagnitude
            < _config.AttackRange * _config.AttackRange + Mathf.Epsilon;

        #endregion
    }
}