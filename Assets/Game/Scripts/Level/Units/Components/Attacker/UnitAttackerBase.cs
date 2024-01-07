namespace Game.Units
{
	using Game.Utilities;
	using Zenject;
	using Game.Configs;
	using UnityEngine;

	public interface IUnitAttacker
	{
		bool IsReadyToAttack { get; }
		bool IsTargetClose(IUnitFacade target);
		void Attack(IUnitFacade target);
	}

	public abstract class UnitAttackerBase : ControllerBase, IInitializable, IUnitAttacker
	{
		[Inject] IUnitView _view;
		[Inject] private UnitData _unitData;
		[Inject] UnitConfig _unitConfig;

		protected ITimer AtackTimer;
		protected float CurrentDamage;

		public virtual void Initialize()
		{
			AtackTimer = new Timer();
			CurrentDamage = _unitConfig.Damage + _unitConfig.DamagePowerMultiplier * _unitData.Power;
		}

		#region IUnitAttacker

		public bool IsReadyToAttack => AtackTimer.IsReady;

		public abstract void Attack(IUnitFacade target);

		public bool IsTargetClose(IUnitFacade target) =>
			target != null &&
			target.IsDead == false &&
			(_view.Transform.position - target.Transform.position).sqrMagnitude <
			_unitConfig.AttackRange * _unitConfig.AttackRange + Mathf.Epsilon;

		#endregion
	}
}