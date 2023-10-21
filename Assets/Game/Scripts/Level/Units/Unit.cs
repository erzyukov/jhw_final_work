namespace Game.Units
{
	using UnityEngine;

	public interface IUnit
	{
		Vector2Int Position { get; set; }
		void SetViewParent(Transform tranform);
		Unit.Kind GetKind();
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
		
		public Vector2Int Position { get; set; }

		public void SetViewParent(Transform transform) =>
			_unitView.SetParent(transform);

		public Kind GetKind() => 
			_kind;

		public enum Kind
		{
			HeavyTank,
			LightTank,
			Howitzer,
			Support,
		}
	}
}