namespace Game.Units
{
	using Game.Configs;
	using Game.Utilities;
	using System;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public interface IUnitFsm
	{
		ReactiveCommand<UnitState> StateChanged { get; }
		void EnterBattle();
		void Reset();
	}

	public class UnitFsm : UnitFsmBase, IUnitFsm, IInitializable, IDisposable, IFixedTickable
	{
		[Inject] private IUnitTargetFinder _targetFinder;
		[Inject] private IUnitHealth _health;
		[Inject] private IUnitMover _mover;
		[Inject] private IUnitAttacker _attacker;
		[Inject] private IUnitView _view;
		[Inject] private IUnitBuilder _builder;
		[Inject] private IUnitPosition _unitPosition;
		[Inject] private UnitConfig _config;

		protected const float NormalizedTransitionDuration = 0.5f;

		readonly private CompositeDisposable _disposable = new CompositeDisposable();

		private int IdleAnimation => Animator.StringToHash("Idle");
		private int AttackTrigger => Animator.StringToHash("Attack");
		private int IsMoveParameter => Animator.StringToHash("IsMove");
		private int DieTrigger => Animator.StringToHash("Die");
		private int IsAimingParameter => Animator.StringToHash("IsAiming");
		private int IsDeadParameter => Animator.StringToHash("IsDead");
		private int AimParameter => Animator.StringToHash("Aim");
		private int IdleOffsetParameter => Animator.StringToHash("IdleOffset");
		private Animator Animator => _builder.UnitRenderer.Animator;

		public void Initialize()
		{
			_builder.UnitRenderer.AnimationEventsCatcher.Hited
				.Where(_ => State == UnitState.StartAttack)
				.Subscribe(_ => HitTarget())
				.AddTo(_disposable);

			_builder.UnitRenderer.AnimationEventsCatcher.AttackAnimationCompleted
				.Where(_ => State == UnitState.StartAttack)
				.Subscribe(_ => Transition(UnitState.PrepareAttack))
				.AddTo(_disposable);

			_health.Died
				.Subscribe(_ => Transition(UnitState.Dying))
				.AddTo(_disposable);

			_builder.UnitRenderer.AnimationEventsCatcher.DeathAnimationCompleted
				.Where(_ => State == UnitState.Dying)
				.Subscribe(_ => Transition(UnitState.Dead))
				.AddTo(_disposable);
		}

		public virtual void Dispose() => _disposable.Dispose();

		#region IUnitFsm

		public ReactiveCommand<UnitState> StateChanged { get; } = new ReactiveCommand<UnitState>();

		public void EnterBattle() =>
			Transition(UnitState.SearchTarget);

		public void Reset() =>
			Transition(UnitState.Idle);

		#endregion

		#region SimpleFsmBase

		protected override void Transition(UnitState state)
		{
			#region Debug

			if (_config.IsDebug)
			{
				string target = "none";
				try { target = _targetFinder.Target.Transform.name; }
				catch { }
				Debug.LogWarning($"State changed: {state} | target = {target}");
			}

			#endregion

			base.Transition(state);
			StateChanged.Execute(state);
		}

		protected override void StateTransitions()
		{
			switch (State)
			{
				case UnitState.None:
					Transition(UnitState.Idle);
					break;

				case UnitState.TargetLost:
					Transition(UnitState.SearchTarget);
					break;

				case UnitState.SearchTarget:
					if (_targetFinder.HasTarget)
						Transition(UnitState.TargetFound);
					
					break;

				case UnitState.TargetFound:
					Transition(UnitState.MoveToTarget);
					break;

				case UnitState.MoveToTarget:
					if (_targetFinder.HasTarget == false)
						Transition(UnitState.TargetLost);
					else if (_mover.IsMoving == false)
						Transition(UnitState.PrepareAttack);

					break;

				case UnitState.PrepareAttack:
					if (_attacker.IsTargetClose(_targetFinder.Target) == false)
						Transition(UnitState.MoveToTarget);
					else if (_attacker.IsReadyToAttack)
						Transition(UnitState.StartAttack);

					break;

				case UnitState.StartAttack:
					if (_attacker.IsTargetClose(_targetFinder.Target) == false)
						Transition(UnitState.MoveToTarget);

					break;
			}
		}

		protected override void StateTick()
		{
			switch (State)
			{
				case UnitState.MoveToTarget:
					_mover.ProcessMoveTo(_targetFinder.Target);
					break;
			}
		}

		protected override void StateFixedTick()
		{
			switch (State)
			{
				case UnitState.MoveToTarget:
					_targetFinder.ActualizeTarget();
					break;
			}
		}

		#endregion

		#region UnitFsmBase

		#region Idle State

		protected override void OnEnteredIdle()
		{
			_health.Reset();
			_targetFinder.Reset();
			_mover.Stop();
			_unitPosition.ResetPosition();
			_view.SetActive(true);
			_view.NavMeshAgent.enabled = false;
			Animator.SetFloat(IdleOffsetParameter, UnityEngine.Random.value);
			Animator.Play(IdleAnimation);
		}

		#endregion

		#region SearchTarget State

		protected override void OnEnteredSearchTarget()
		{
			_view.NavMeshAgent.enabled = true;
			_targetFinder.SearchTarget();
		}

		#endregion

		#region MoveToTarget State

		protected override void OnEnteredMoveToTarget()
		{
			Animator.SetBool(IsAimingParameter, false);
			Animator.SetBool(IsMoveParameter, true);
		}

		protected override void OnExitedMoveToTarget()
		{
			Animator.SetBool(IsMoveParameter, false);
		}
		
		#endregion

		#region PrepareAttack State

		protected override void OnEnteredPrepareAttack()
		{
			_mover.LookAt(_targetFinder.Target);
			if (Animator.GetBool(IsAimingParameter) == false)
			{
				Animator.SetTrigger(AimParameter);
				Animator.SetBool(IsAimingParameter, true);
			}
		}

		#endregion

		#region StartAttack State

		protected override void OnEnteredStartAttack()
		{
			Animator.SetTrigger(AttackTrigger);
		}

		#endregion

		#region HitTarget State

		private void HitTarget()
		{
			_attacker.Attack(_targetFinder.Target);
		}

		#endregion

		#region TargetLost State

		protected override void OnEnteredTargetLost()
		{
			_mover.Stop();
			Animator.SetBool(IsAimingParameter, false);
		}

		#endregion

		#region Dying State

		protected override void OnEnteredDying()
		{
			_mover.Stop();
			
			Animator.SetTrigger(DieTrigger);
			Animator.SetBool(IsDeadParameter, true);
			Animator.SetBool(IsAimingParameter, false);
		}

		#endregion

		#region Dead State

		protected override void OnEnteredDead() {}
		
		#endregion

		#endregion
	}
}