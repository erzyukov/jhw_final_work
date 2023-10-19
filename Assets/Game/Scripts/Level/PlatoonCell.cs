namespace Game.Level
{
	using Units;
	using UnityEngine;

	public interface IPlatoonCell
	{
		bool HasUnit { get; }
		void SetUnit(Unit unit);
		void Clear();
	}

	public class PlatoonCell : IPlatoonCell
	{
		private IPlatoonCellView _cellView;
		private Unit _unit;

		public PlatoonCell(IPlatoonCellView cellView, Camera camera)
		{
			cellView.Init(camera);
			_cellView = cellView;
		}

		public bool HasUnit => _unit != null;

		public void SetUnit(Unit unit)
		{
			_unit = unit;
			_unit.SetViewParent(_cellView.UnitPivot);
		}

		public void Clear()
		{
			_unit = null;
		}
	}
}