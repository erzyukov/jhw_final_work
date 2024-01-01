namespace Game.Units
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using UnityEngine;

	public interface IUnitMover
	{
		ReactiveCommand ReachedDestination { get; }
		bool IsMoving { get; }
		void ProcessMoveTo(IUnitFacade target);
        void LookAt(IUnitFacade target);
        void Stop();
	}

	public class UnitMover : ControllerBase, IUnitMover
	{
		[Inject] private IUnitView _unitView;

        #region IUnitMover

        public ReactiveCommand ReachedDestination { get; } = new ReactiveCommand();

		public bool IsMoving { get; private set; }

		public void ProcessMoveTo(IUnitFacade target)
		{
            if (target == null)
			{
				//IsInProcess = false;
				return;
			}

			_unitView.NavMeshAgent.updateRotation = true;
            _unitView.NavMeshAgent.isStopped = false;
			_unitView.NavMeshAgent.SetDestination(target.Transform.position);
			IsMoving = true;

			if (_unitView.NavMeshAgent.hasPath && _unitView.NavMeshAgent.remainingDistance <= _unitView.NavMeshAgent.stoppingDistance + Mathf.Epsilon)
			{
				IsMoving = false;
				ReachedDestination.Execute();
			}
		}

        public void LookAt(IUnitFacade target)
        {
            if (target == null)
                return;

            _unitView.NavMeshAgent.updateRotation = false;
            Vector3 lookPosition = target.Transform.position - _unitView.NavMeshAgent.transform.position;
            _unitView.NavMeshAgent.transform.rotation = Quaternion.LookRotation(lookPosition.WithY(0));
        }

        public void Stop()
		{
			if (_unitView.NavMeshAgent.isActiveAndEnabled)
				_unitView.NavMeshAgent.isStopped = true;
		}

		#endregion
	}
}