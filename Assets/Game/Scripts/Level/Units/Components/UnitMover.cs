namespace Game.Units
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using UnityEngine;

	public interface IUnitMover
	{
		ReactiveCommand ReachedDestination { get; }
		void MoveTo(IUnitFacade target);
		void Stop();
	}

	public class UnitMover : ControllerBase, IUnitMover
	{
		[Inject] private IUnitView _unitView;

		#region IUnitMover

		public ReactiveCommand ReachedDestination { get; } = new ReactiveCommand();

		public void MoveTo(IUnitFacade target)
		{
            if (target == null)
                return;

			_unitView.NavMeshAgent.isStopped = false;

			_unitView.NavMeshAgent.SetDestination(target.Transform.position);

			if (_unitView.NavMeshAgent.hasPath && _unitView.NavMeshAgent.remainingDistance <= _unitView.NavMeshAgent.stoppingDistance + Mathf.Epsilon)
				ReachedDestination.Execute();
		}

		public void Stop()
		{
			if (_unitView.NavMeshAgent.isActiveAndEnabled)
				_unitView.NavMeshAgent.isStopped = true;
		}

		#endregion
	}
}