namespace Game.Units
{
	using Game.Configs;
	using Game.Utilities;
	using UnityEngine;
	using Zenject;

	public interface IUnitBuilder
	{
		IUnitModel Model { get; }

		void SetupUnitView(int index);
	}

	public class UnitBuilder : IUnitBuilder
	{
		[Inject] private IUnitView _unitView;
		[Inject] private UnitsConfig _unitsConfig;
		[Inject] private UnitConfig _unitConfig;
		[Inject] private UnitGrade _unitGrade;
		[Inject] private UnitModel.Factory _unitModelFactory;

		private IUnitModel _model;

		public void OnInstantiated()
		{
			SetupUnitView(0);
		}

		#region IUnitBuilder

		public IUnitModel Model => _model;

		public void SetupUnitView(int index)
		{
			_model = _unitModelFactory.Create(_unitGrade.Prefab);
			_unitView.ModelContainer.DestroyChildren();
			_model.Transform.SetParent(_unitView.ModelContainer, false);

			_unitView.NavMeshAgent.speed = _unitsConfig.Speed;
			_unitView.NavMeshAgent.stoppingDistance = _unitConfig.AttackRange;

			float uiHealthHeight = 
                _model.Renderer.bounds.size.y * _model.Renderer.transform.localScale.y * _model.Renderer.transform.parent.localScale.y + 
                _unitsConfig.UiHealthIndent;
			_unitView.SetModelHeight(uiHealthHeight);
			_unitView.SetModelRendererTransform(_model.RendererTransform);

			#region Debug

			_unitView.Transform.name += $" - {Random.Range(1000, 9999)}";

			#endregion
		}

		#endregion
	}
}