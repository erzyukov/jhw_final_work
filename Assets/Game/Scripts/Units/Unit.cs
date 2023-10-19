namespace Game.Units
{
	using UnityEngine;

	public class Unit
	{
		private Kind _kind;
		private UnitView _unitView;

		public Unit(Kind kind, UnitView unitView)
		{
			_kind = kind;
			_unitView = unitView;
		}

		public void SetViewParent(Transform tranform)
		{
			_unitView.transform.SetParent(tranform, false);
		}

		public Kind GetKind() => _kind;

		public enum Kind
		{
			HeavyTank,
			LightTank,
			Howitzer,
			Support,
		}
	}
}