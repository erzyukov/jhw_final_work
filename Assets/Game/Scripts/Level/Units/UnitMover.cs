namespace Game.Units
{
	using Game.Utilities;
	using Zenject;
	using UniRx;

	public class UnitMover : ControllerBase, IInitializable, IFixedTickable
	{
		[Inject] private IUnitTargetFinder _targetFinder;
		[Inject] private IUnitView _unitView;
		[Inject] private IUnitHealth _unitHealth;

		IUnitFacade _target;

		public void Initialize()
		{
			_targetFinder.TargetFound
				.Subscribe(OnTargetFoundHandler)
				.AddTo(this);

			_targetFinder.TargetLost
				.Subscribe(_ => OnTargetLostHandler())
				.AddTo(this);

			_unitHealth.Died
				.Subscribe(_ => _target = null)
				.AddTo(this);
		}

		public void FixedTick()
		{
			if (_target != null)
			{
				_unitView.NavMeshAgent.SetDestination(_target.Transform.position);
			}
		}

		private void OnTargetFoundHandler(IUnitFacade target)
		{
			_target = target;
			_unitView.NavMeshAgent.isStopped = false;
		}

		private void OnTargetLostHandler()
		{
			if (_target == null)
				return;

			_unitView.NavMeshAgent.isStopped = true;
			_target = null;
		}
	}
}