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
		void Transition(UnitState state);
		void Reset();
	}

	public class UnitFsm : UnitFsmBase, IUnitFsm, IInitializable, IDisposable
	{
		[Inject] private IUnitTargetFinder _targetFinder;
		[Inject] private IUnitHealth _health;
		[Inject] private IUnitMover _mover;
		[Inject] private IUnitAttacker _attacker;
		[Inject] private IUnitView _view;
		[Inject] private IUnitBuilder _builder;
		[Inject] private UnitConfig _config;
		[Inject] private TimingsConfig _timingsConfig;

		readonly private CompositeDisposable _disposable = new CompositeDisposable();

		private IUnitFacade _target;
		private UnitState _hitTargetWithAnimationState;
		private UnitState _finishAttackWithAnimationState;

		private int AttackTrigger => Animator.StringToHash("Attack");
		private int IsMoveParameter => Animator.StringToHash("IsMove");
		private int DieTrigger => Animator.StringToHash("Die");
		private int IsAimingParameter => Animator.StringToHash("IsAiming");
		private int AimParameter => Animator.StringToHash("Aim");
		private Animator Animator => _builder.Model.Animator;

		public void Initialize()
		{
			_hitTargetWithAnimationState = (Animator != null) ? UnitState.StartAttack : UnitState.HitTarget;
			_finishAttackWithAnimationState = (Animator != null) ? UnitState.FinishAttack : UnitState.PrepareAttack;

			InitializeSubscribes();
		}

		public virtual void Dispose() => _disposable.Dispose();

		private void InitializeSubscribes()
		{
			_targetFinder.TargetFound
				.Subscribe(target =>
				{
					_target = target;
					Transition(UnitState.TargetFound);
				})
				.AddTo(_disposable);

			_mover.ReachedDestination
				.Where(_ => State != UnitState.PrepareAttack)
				.Subscribe(_ => Transition(UnitState.PrepareAttack))
				.AddTo(_disposable);

			if (_builder.Model.AnimationEventsCatcher != null)
			{
				_builder.Model.AnimationEventsCatcher.Hited
					.Where(_ => State == UnitState.StartAttack)
					.Subscribe(_ => Transition(UnitState.HitTarget))
					.AddTo(_disposable);

				_builder.Model.AnimationEventsCatcher.AttackAnimationCompleted
					.Where(_ => State == UnitState.FinishAttack)
					.Subscribe(_ => Transition(UnitState.PrepareAttack))
					.AddTo(_disposable);

				_builder.Model.AnimationEventsCatcher.DeathAnimationCompleted
					.Where(_ => State == UnitState.Dying)
					.Subscribe(_ => Transition(UnitState.Dead))
					.AddTo(_disposable);
			}

			_attacker.AttackRangeBroken
				.Subscribe(_ => Transition(UnitState.MoveToTarget))
				.AddTo(_disposable);

			_health.Died
				.Subscribe(_ => Transition(UnitState.Dying))
				.AddTo(_disposable);
		}

		#region SimpleFsmBase

		public ReactiveCommand<UnitState> StateChanged { get; } = new ReactiveCommand<UnitState>();

		public override void Transition(UnitState state)
		{
			#region Debug

			if (_config.IsDebug)
			{
				string target = "none";
				try { target = _target.Transform.name; }
				catch { }
				Debug.LogWarning($"State changed: {state} | target = {target}");
			}

			#endregion

			base.Transition(state);
			StateChanged.Execute(state);
		}

		public void Reset()
		{
			Transition(UnitState.Idle);
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

				case UnitState.TargetFound:
					Transition(UnitState.MoveToTarget);
					break;

				case UnitState.PrepareAttack:
					if (_attacker.CanAttack(_targetFinder.Target))
						Transition(_hitTargetWithAnimationState);

					break;

				case UnitState.MoveToTarget:
					if (_targetFinder.HasTarget == false)
						Transition(UnitState.TargetLost);

					break;

				case UnitState.Dying:
					if (Animator == null)
						Transition(UnitState.Dead);

					break;
			}
		}

		protected override void StateTick()
		{
			switch (State)
			{
				case UnitState.MoveToTarget:
					_mover.MoveTo(_targetFinder.Target);
					break;

				case UnitState.PrepareAttack:
				case UnitState.StartAttack:
				case UnitState.HitTarget:
				case UnitState.FinishAttack:
					_attacker.ProcessTargetTracking(_targetFinder.Target);
					break;
			}
		}

		#endregion

		#region UnitFsmBase

		protected override void OnEnteredIdle()
		{
			_health.Reset();
			_targetFinder.Reset();
			_mover.Stop();
			_view.ResetPosition();
			_view.SetActive(true);
			_view.NavMeshAgent.enabled = false;
		}

		protected override void OnEnteredSearchTarget()
		{
			_view.NavMeshAgent.enabled = true;
			_targetFinder.SearchTarget();
		}

		protected override void OnEnteredMoveToTarget()
		{
			if (Animator != null)
				Animator.SetBool(IsMoveParameter, true);
		}

		protected override void OnExitedMoveToTarget()
		{
			if (Animator != null)
				Animator.SetBool(IsMoveParameter, false);
		}

		protected override void OnEnteredPrepareAttack()
		{
			_mover.LookAt(_targetFinder.Target);
			if (Animator != null && Animator.GetBool(IsAimingParameter) == false)
			{
				Animator.SetTrigger(AimParameter);
				Animator.SetBool(IsAimingParameter, true);
			}
		}

		protected override void OnEnteredStartAttack()
		{
			Animator.SetTrigger(AttackTrigger);
		}

		protected override void OnEnteredHitTarget()
		{
			_attacker.Attack(_targetFinder.Target);
			Transition(_finishAttackWithAnimationState);
		}

		protected override void OnEnteredTargetLost()
		{
			_mover.Stop();
			if (Animator != null)
				Animator.SetBool(IsAimingParameter, false);
		}

		protected override void OnEnteredDying()
		{
			_mover.Stop();

			if (Animator != null)
			{
				Animator.SetBool(IsAimingParameter, false);
				Animator.SetTrigger(DieTrigger);
			}
		}

		protected override void OnEnteredDead()
		{
			Observable.Timer(TimeSpan.FromSeconds(_timingsConfig.UnitDeathVanishDelay))
				.Subscribe(_ => _view.SetActive(false))
				.AddTo(_disposable);
		}

		#endregion
	}
}