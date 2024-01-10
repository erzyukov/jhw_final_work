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
		[Inject] private IUnitData _unitData;
		[Inject] private UnitsConfig _unitsConfig;
		[Inject] private UnitConfig _unitConfig;
		[Inject] private UnitRenderer.Factory _unitRendererFactory;

		private IUnitRenderer _unitRenderer;

		public void OnInstantiated(UnitCreateData unitCreateData)
		{
			SetupUnit(unitCreateData);
		}

		void SetupUnit(UnitCreateData unitCreateData)
		{
			GameObject prefab = _unitConfig.GradePrefabs[unitCreateData.GradeIndex];
			_unitRenderer = _unitRendererFactory.Create(prefab);
			_unitView.RendererContainer.DestroyChildren();
			_unitRenderer.Transform.SetParent(_unitView.RendererContainer, false);

			_unitView.NavMeshAgent.speed = _unitsConfig.Speed;
			_unitView.NavMeshAgent.stoppingDistance = _unitConfig.AttackRange;
			_unitView.SetModelRendererTransform(_unitRenderer.RendererTransform);
			
			float uiHealthHeight =
				_unitRenderer.Renderer.bounds.size.y * _unitRenderer.Renderer.transform.localScale.y * _unitRenderer.Renderer.transform.parent.localScale.y +
				_unitsConfig.UiHealthIndent;

			_unitData.Init(unitCreateData.GradeIndex, unitCreateData.IsHero, uiHealthHeight);
			_unitData.Power.Value = unitCreateData.Power;

			#region Debug

			_unitView.Transform.name += $" - {Random.Range(1000, 9999)}";

			#endregion
		}

		#region IUnitBuilder

		public IUnitRenderer UnitRenderer => _unitRenderer;

		#endregion
	}
}