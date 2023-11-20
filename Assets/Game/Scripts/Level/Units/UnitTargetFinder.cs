namespace Game.Units
{
	using Game.Core;
	using Game.Field;
	using Game.Utilities;
	using System.Linq;
	using UniRx;
	using Zenject;

	public interface IUnitTargetFinder
	{
		ReactiveCommand<IUnitFacade> TargetFound { get; }
	}

	public class UnitTargetFinder : ControllerBase, IUnitTargetFinder, IInitializable, ITickable
	{
		[Inject] private IGameCycle _levelCycle;
		[Inject] private IFieldFacade[] _fields;
		[Inject] private UnitFacade _unitFacade;

		private IUnitFacade _target;
		private IFieldFacade _alliedField;
		private IFieldFacade _enemyField;

		public void Initialize()
		{
			_levelCycle.State
				.Where(state => state == GameState.BattleStage)
				.Subscribe(_ => OnBattleStageHandler())
				.AddTo(this);
		}

		public void Tick()
		{
			if (_target == null && _enemyField != null && _enemyField.Units.Count != 0)
			{
				SelectTarget();
			}
		}

		#region IUnitTargetFinder

		public ReactiveCommand<IUnitFacade> TargetFound { get; } = new ReactiveCommand<IUnitFacade>();

		#endregion

		private void OnBattleStageHandler()
		{
			InitFields();
			SelectTarget();
        }

		private void SelectTarget()
		{
			_target = _enemyField.Units
				.OrderBy(u => (_unitFacade.Transform.position - u.Transform.position).sqrMagnitude)
				.FirstOrDefault();

			if (_target != null)
				TargetFound.Execute(_target);
		}

		private void InitFields()
		{
			for (int i = 0; i < _fields.Length; i++)
			{
				if (_fields[i].HasUnit(_unitFacade))
					_alliedField = _fields[i];

				if (_fields[i].HasUnit(_unitFacade) == false)
					_enemyField = _fields[i];
			}
		}
	}
}
