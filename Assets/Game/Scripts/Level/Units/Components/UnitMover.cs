namespace Game.Units
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using UnityEngine;
	using DG.Tweening;
	using Game.Configs;

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
		[Inject] private UnitsConfig _unitsConfig;

        #region IUnitMover

        public ReactiveCommand ReachedDestination { get; } = new ReactiveCommand();

		public bool IsMoving { get; private set; }

		public void ProcessMoveTo(IUnitFacade target)
		{
            if (target == null)
				return;

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

			const float FullAngle = 360;
			const float HalfFullAngle = 180;

			_unitView.NavMeshAgent.updateRotation = false;
            Vector3 lookPosition = target.Transform.position - _unitView.NavMeshAgent.transform.position;
			Quaternion startRotation = _unitView.NavMeshAgent.transform.rotation;
			Quaternion targetRotation = Quaternion.LookRotation(lookPosition.WithY(0));
			float deltaAngle = Mathf.Abs(targetRotation.eulerAngles.y - startRotation.eulerAngles.y);
			deltaAngle = (deltaAngle > HalfFullAngle) ? FullAngle - deltaAngle : deltaAngle;
			float time = deltaAngle * _unitsConfig.RotationSpeed / FullAngle;

			DOVirtual.Float(0, 1, time, t =>
			{
				_unitView.NavMeshAgent.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
			});
        }

        public void Stop()
		{
			if (_unitView.NavMeshAgent.isActiveAndEnabled)
				_unitView.NavMeshAgent.isStopped = true;
		}

		#endregion
	}
}