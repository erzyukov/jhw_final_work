namespace Game.Units
{
	using Game.Configs;
	using Game.Utilities;
	using Zenject;

	public interface IUnitBuilder
	{
		IUnitModel View { get; }

		void SetupUnitView(int index);
	}

	public class UnitBuilder : IUnitBuilder
	{
		[Inject] private UnitsConfig _unitsConfig;
		[Inject] private UnitConfig _unitConfig;
		[Inject] private UnitModel.Factory _unitModelFactory;
		[Inject] private IUnitView _unitView;

		private IUnitModel _view;

		public void OnInstantiated()
		{
			SetupUnitView(0);
		}

		#region IUnitBuilder

		public IUnitModel View => _view;

		public void SetupUnitView(int index)
		{
			_view = _unitModelFactory.Create(_unitConfig.Grades[index].Prefab);
			_unitView.ModelContainer.DestroyChildren();
			_view.Transform.SetParent(_unitView.ModelContainer, false);

			_unitView.NavMeshAgent.speed = _unitsConfig.Speed;
			_unitView.NavMeshAgent.stoppingDistance = _unitConfig.AttackRange;
		}

		#endregion
	}
}