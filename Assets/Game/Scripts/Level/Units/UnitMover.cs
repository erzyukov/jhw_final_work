namespace Game.Units
{
	using Game.Utilities;
	using Zenject;
	using UniRx;

	public interface IUnitMover
	{

	}

	public class UnitMover : ControllerBase, IUnitMover, IInitializable, IFixedTickable
	{
		[Inject] private IUnitTargetFinder _targetFinder;
		[Inject] private IUnitView _unitView;

		IUnitFacade _target;

		public void Initialize()
		{
			_targetFinder.TargetFound
				.Subscribe(OnTargetFoundHandler)
				.AddTo(this);

			_targetFinder.TargetLost
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
		}
	}
}