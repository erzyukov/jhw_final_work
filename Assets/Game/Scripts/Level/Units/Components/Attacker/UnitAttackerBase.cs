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

	public abstract class UnitAttackerBase : ControllerBase, IInitializable, IUnitAttacker
	{
		[Inject] IUnitView _view;
		[Inject] private Species _species;
		[Inject] private IGameUpgrades _gameUpgrades;
		[Inject] UnitGrade _grade;
		[Inject] UnitConfig _config;

		protected ITimer AtackTimer;
		protected float CurrentDamage;

		public virtual void Initialize()
		{
			AtackTimer = new Timer();
			CurrentDamage = _gameUpgrades.GetUnitDamage(_species) * _grade.DamageMultiplier;
		}

		#region IUnitAttacker

		public bool IsReadyToAttack => AtackTimer.IsReady;

		public abstract void Attack(IUnitFacade target);

		public bool IsTargetClose(IUnitFacade target) =>
			target != null &&
			target.IsDead == false &&
			(_view.Transform.position - target.Transform.position).sqrMagnitude <
			_config.AttackRange * _config.AttackRange + Mathf.Epsilon;

		#endregion
	}
}