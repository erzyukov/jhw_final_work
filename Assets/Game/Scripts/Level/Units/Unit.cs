namespace Game.Units
{
	using Game.Configs;
	using UnityEngine;

	public interface IUnit
	{
		void SetViewParent(Transform tranform);
		Unit.Kind GetKind();
		void SetIgnoreRaycast(bool value);
	}

	public class Unit : IUnit
	{
		protected Kind _kind;
		protected IUnitView _unitView;
		protected UnitConfig _config;

		public Unit(Kind kind, IUnitView unitView)
		public Unit(Kind kind, IUnitView unitView, UnitConfig config)
		{
			_kind = kind;
			_unitView = unitView;
			_config = config;
		}
		
		public void SetViewParent(Transform transform) =>
			_unitView.SetParent(transform);

		public Kind GetKind() => 
			_kind;

		public void SetIgnoreRaycast(bool value) => 
			_unitView.SetIgnoreRaycast(value);

		public enum Kind
		{
			HeavyTank,
			LightTank,
			Howitzer,
			Support,
		}
	}
}