namespace Game.Units
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Configs;
	using UnityEngine;
	using Game.Core;

	public interface IUnitAttacker
	{
		ReactiveCommand AttackRangeBroken { get; }
        bool CanAttack(IUnitFacade target);
        void Attack(IUnitFacade target);
        void TryAttack(IUnitFacade target);
        void ProcessTargetTracking(IUnitFacade target);
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

		public ReactiveCommand AttackRangeBroken { get; } = new ReactiveCommand();

        public bool CanAttack(IUnitFacade target) =>
            target != null && target.IsDead == false && _atackTimer.IsReady && IsTargetClose(target);

        public void Attack(IUnitFacade target)
        {
            if (CanAttack(target) == false)
                return;

            target.TakeDamage(_currentDamage);
            _atackTimer.Set(_grade.AttackDelay);
        }

        public void ProcessTargetTracking(IUnitFacade target)
        {
			if (IsTargetClose(target) == false)
				AttackRangeBroken.Execute();
        }

        public void TryAttack(IUnitFacade target)
		{
			if (IsTargetClose(target) == false)
			{
				AttackRangeBroken.Execute();

				return;
			}

			if (_atackTimer.IsReady == false)
				return;

			target.TakeDamage(_currentDamage);
			_atackTimer.Set(_grade.AttackDelay);
		}

		#endregion

		private bool IsTargetClose(IUnitFacade target) =>
            target != null && 
            target.IsDead != true &&
            (_view.Transform.position - target.Transform.position).sqrMagnitude 
			< _config.AttackRange * _config.AttackRange + Mathf.Epsilon;
	}
}