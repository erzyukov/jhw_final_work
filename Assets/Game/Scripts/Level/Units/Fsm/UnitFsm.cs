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
		void Transition(UnitState state);
	}

	public class UnitFsm : UnitFsmBase, IUnitFsm, IInitializable, IDisposable
	{
		[Inject] private IUnitTargetFinder _targetFinder;
		[Inject] private IUnitHealth _health;
		[Inject] private IUnitMover _mover;
		[Inject] private IUnitAttacker _attacker;
		[Inject] private IUnitView _view;
		[Inject] private UnitConfig _config;

		readonly private CompositeDisposable _disposable = new CompositeDisposable();

		private IUnitFacade _target;

		public void Initialize()
		{
			_targetFinder.TargetFound
				.Subscribe(target =>
				{
					_target = target;
					Transition(UnitState.TargetFound);
				})
				.AddTo(_disposable);

			_mover.ReachedDestination
				.Subscribe(_ => Transition(UnitState.Attack))
				.AddTo(_disposable);

			_attacker.AttackRangeBroken
				.Subscribe(_ => Transition(UnitState.MoveToTarget))
				.AddTo(_disposable);

			_health.Died
				.Subscribe(_ => Transition(UnitState.Died))
				.AddTo(_disposable);
		}

		public virtual void Dispose() => _disposable.Dispose();

		#region SimpleFsmBase

		public override void Transition(UnitState state)
		{
			#region Debug

			if (_config.IsDebug)
			{
				string target = (_target == null) ? "none" : _target.Transform.name;
				Debug.LogWarning($"State changed: {state} | target = {target}");
			}

			#endregion

			base.Transition(state);
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

				case UnitState.MoveToTarget:
				case UnitState.Attack:
					if (_targetFinder.HasTarget == false)
						Transition(UnitState.TargetLost);
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

				case UnitState.Attack:
					_attacker.TryAttack(_targetFinder.Target);
					break;
			}
		}

		#endregion

		#region UnitFsmBase

		protected override void OnEnterIdleHandler()
		{
			_health.Reset();
			_targetFinder.Reset();
			_mover.Stop();
			_view.ResetPosition();
			_view.SetActive(true);
			_view.NavMeshAgent.enabled = false;
		}

		protected override void OnEnterSearchTargetHandler()
		{
			_view.NavMeshAgent.enabled = true;
			_targetFinder.SearchTarget();
		}

		protected override void OnEnterTargetLostHandler()
		{
			_mover.Stop();
		}

		protected override void OnEnterDiedHandler()
		{
			_mover.Stop();
			_view.SetActive(false);
		}

		#endregion
	}
}