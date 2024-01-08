namespace Game.Units
{
	using Game.Configs;
	using Game.Utilities;
	using UnityEngine;
	using Zenject;

	public interface IUnitBuilder
	{
		IUnitRenderer UnitRenderer { get; }
	}

	public class UnitBuilder : IUnitBuilder
	{
		[Inject] private IUnitView _unitView;
		[Inject] private UnitsConfig _unitsConfig;
		[Inject] private UnitConfig _unitConfig;
		[Inject] private UnitCreateData _unitCreateData;
		[Inject] private UnitRenderer.Factory _unitRendererFactory;

		private IUnitRenderer _unitRenderer;

		public void OnInstantiated()
		{
			SetupUnit();
		}

		void SetupUnit()
		{
			GameObject prefab = _unitConfig.GradePrefabs[_unitCreateData.GradeIndex];
			_unitRenderer = _unitRendererFactory.Create(prefab);
			_unitView.ModelContainer.DestroyChildren();
			_unitRenderer.Transform.SetParent(_unitView.ModelContainer, false);

			_unitView.NavMeshAgent.speed = _unitsConfig.Speed;
			_unitView.NavMeshAgent.stoppingDistance = _unitConfig.AttackRange;

			float uiHealthHeight =
				_unitRenderer.Renderer.bounds.size.y * _unitRenderer.Renderer.transform.localScale.y * _unitRenderer.Renderer.transform.parent.localScale.y +
				_unitsConfig.UiHealthIndent;
			_unitView.SetModelHeight(uiHealthHeight);
			_unitView.SetModelRendererTransform(_unitRenderer.RendererTransform);

			#region Debug

			_unitView.Transform.name += $" - {Random.Range(1000, 9999)}";

			#endregion
		}

		#region IUnitBuilder

		public IUnitRenderer UnitRenderer => _unitRenderer;

		#endregion
	}
}