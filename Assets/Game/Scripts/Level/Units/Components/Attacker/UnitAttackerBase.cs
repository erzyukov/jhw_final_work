namespace Game.Units
{
	using Game.Utilities;
	using Zenject;
	using Game.Configs;
	using UnityEngine;
	using UniRx;

	public interface IUnitAttacker
	{
		ReactiveCommand Attacking { get; }
		bool IsReadyToAttack { get; }
		bool IsTargetClose(IUnitFacade target);
		void Attack(IUnitFacade target);
	}

	public abstract class UnitAttackerBase : ControllerBase, IInitializable, IUnitAttacker
	{
		[Inject] protected IUnitView View;
		[Inject] private IUnitData _unitData;
		[Inject] private UnitConfig _unitConfig;

		protected ITimer AtackTimer;
		protected float CurrentDamage;

		public virtual void Initialize()
		{
			AtackTimer = new Timer();
			CurrentDamage = _unitConfig.Damage + _unitConfig.DamagePowerMultiplier * _unitData.Power.Value;
		}

		#region IUnitAttacker

		public ReactiveCommand Attacking { get; } = new ReactiveCommand();

		public bool IsReadyToAttack => AtackTimer.IsReady;

		public virtual void Attack(IUnitFacade target) =>
			Attacking.Execute();

		public bool IsTargetClose(IUnitFacade target) =>
			target != null &&
			target.IsDead == false &&
			(View.Transform.position - target.Transform.position).sqrMagnitude <
			_unitConfig.AttackRange * _unitConfig.AttackRange + Mathf.Epsilon;

		#endregion
	}
}