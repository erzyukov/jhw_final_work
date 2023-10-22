namespace Game.Units
{
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

		public Unit(Kind kind, IUnitView unitView)
		{
			_kind = kind;
			_unitView = unitView;
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