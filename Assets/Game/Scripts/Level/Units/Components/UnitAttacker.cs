namespace Game.Units
{
	using Game.Utilities;
	using Zenject;
	using Game.Configs;
	using UnityEngine;
	using Game.Core;

	public interface IUnitAttacker
	{
		bool IsReadyToAttack { get; }
		bool IsTargetClose(IUnitFacade target);
        void Attack(IUnitFacade target);
	}

    // TODO: Add base attacker class

	public class UnitAttacker : ControllerBase, IInitializable, IUnitAttacker
	{
		[Inject] IUnitView _view;
		[Inject] private Species _species;
		[Inject] private IGameUpgrades _gameUpgrades;
		[Inject] UnitGrade _grade;
		[Inject] UnitConfig _config;

		private ITimer _atackTimer;
		private float _currentDamage;

		public void Initialize()
		{
			_atackTimer = new Timer();
			_currentDamage = _gameUpgrades.GetUnitDamage(_species) * _grade.DamageMultiplier;
		}

		#region IUnitAttacker

		public bool IsReadyToAttack => _atackTimer.IsReady;

        public void Attack(IUnitFacade target)
        {
			target.TakeDamage(_currentDamage);
            _atackTimer.Set(_grade.AttackDelay);
        }

		public bool IsTargetClose(IUnitFacade target) =>
            target != null && 
            target.IsDead == false &&
            (_view.Transform.position - target.Transform.position).sqrMagnitude < 
			_config.AttackRange * _config.AttackRange + Mathf.Epsilon;

		#endregion
	}
}