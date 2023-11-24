namespace Game.Units
{
	using Game.Configs;
	using Game.Utilities;
	using UnityEngine;
	using Zenject;

	public interface IUnitBuilder
	{
		IUnitModel View { get; }

		void SetupUnitView(int index);
	}

	public class UnitBuilder : IUnitBuilder
	{
		[Inject] private IUnitView _unitView;
		[Inject] private UnitsConfig _unitsConfig;
		[Inject] private UnitConfig _unitConfig;
		[Inject] private UnitGrade _unitGrade;
		[Inject] private UnitModel.Factory _unitModelFactory;

		private IUnitModel _view;

		public void OnInstantiated()
		{
			SetupUnitView(0);
		}

		#region IUnitBuilder

		public IUnitModel View => _view;

		public void SetupUnitView(int index)
		{
			_view = _unitModelFactory.Create(_unitGrade.Prefab);
			_unitView.ModelContainer.DestroyChildren();
			_view.Transform.SetParent(_unitView.ModelContainer, false);

			_unitView.NavMeshAgent.speed = _unitsConfig.Speed;
			_unitView.NavMeshAgent.stoppingDistance = _unitConfig.AttackRange;

			#region Debug

			_unitView.Transform.name += $" - {Random.Range(1000, 9999)}";

			#endregion
		}

		#endregion
	}
}