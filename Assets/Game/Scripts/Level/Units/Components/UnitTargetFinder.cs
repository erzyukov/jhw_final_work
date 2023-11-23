namespace Game.Units
{
	using Game.Field;
	using Game.Utilities;
	using System;
	using System.Linq;
	using UniRx;
	using Zenject;

	public interface IUnitTargetFinder
	{
		ReactiveCommand<IUnitFacade> TargetFound { get; }
		ReactiveCommand TargetLost { get; }
		bool HasTarget { get; }
		IUnitFacade Target { get; }
		void SearchTarget();
		void Reset();
	}

	public class UnitTargetFinder : ControllerBase, IUnitTargetFinder
	{
		[Inject] private IFieldFacade[] _fields;
		[Inject] private UnitFacade _unitFacade;

		private IUnitFacade _target;
		private IFieldFacade _alliedField;
		private IFieldFacade _enemyField;
		private IDisposable _targetDisposable;

		#region IUnitTargetFinder

		public ReactiveCommand<IUnitFacade> TargetFound { get; } = new ReactiveCommand<IUnitFacade>();

		public ReactiveCommand TargetLost { get; } = new ReactiveCommand();

		public bool HasTarget => _target != null;
		
		public IUnitFacade Target => _target;

		public void SearchTarget()
		{
			InitFields();
			SelectTarget();
		}

		public void Reset()
		{
			_target = null;
			_targetDisposable?.Dispose();
		}

		#endregion

		private void SelectTarget()
		{
			if (_enemyField == null || _enemyField.Units.Count == 0)
				return;

			_target = _enemyField.Units
				.OrderBy(u => (_unitFacade.Transform.position - u.Transform.position).sqrMagnitude)
				.FirstOrDefault();

			if (_target != null)
			{
				TargetFound.Execute(_target);

				_targetDisposable = _target.Died
					.Subscribe(_ => OnTargetDiedHandler());
			}
		}

		private void InitFields()
		{
			// TODO: refact, how we can take out define allied field?
			for (int i = 0; i < _fields.Length; i++)
			{
				if (_fields[i].HasUnit(_unitFacade))
					_alliedField = _fields[i];

				if (_fields[i].HasUnit(_unitFacade) == false)
					_enemyField = _fields[i];
			}
		}

		private void OnTargetDiedHandler()
		{
			TargetLost.Execute();
			Reset();
		}
	}
}
