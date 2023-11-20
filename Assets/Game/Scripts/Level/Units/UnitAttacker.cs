namespace Game.Units
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Configs;

	public class UnitAttacker : ControllerBase, IInitializable, IFixedTickable
	{
		[Inject] IUnitTargetFinder _targetFinder;
		[Inject] IUnitView _view;
		[Inject] UnitGrade _grade;
		[Inject] UnitConfig _config;

		private IUnitFacade _target;
		private ITimer _atackTimer;

		public void Initialize()
		{
			_atackTimer = new Timer();

			_targetFinder.TargetFound
				.Subscribe(OnTargetFoundHandler)
				.AddTo(this);

			_targetFinder.TargetLost
				.Subscribe(_ => _target = null)
				.AddTo(this);
		}

		public void FixedTick()
		{
			if (_target == null)
				return;

			if (IsTargetClose() == false)
				return;

			if (_atackTimer.IsReady == false)
				return;

			AtackTarget();
		}

		private bool IsTargetClose() =>
			(_view.Transform.position - _target.Transform.position).sqrMagnitude < _config.AttackRange * _config.AttackRange;

		private void OnTargetFoundHandler(IUnitFacade target)
		{
			_atackTimer.Set(_grade.AttackDelay);
			_target = target;
		}

		private void AtackTarget()
		{
			_target.TakeDamage(_grade.Damage);
			_atackTimer.Set(_grade.AttackDelay);
		}
	}
}