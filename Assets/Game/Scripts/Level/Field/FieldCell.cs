using Game.Units;
using UnityEngine;

namespace Game.Field
{
	public interface IFieldCell
	{
		bool HasUnit { get; }
		IUnit Unit { get; }
		Vector2Int Position { get; }
		void SetUnit(IUnit unit);
		void Clear();
	}

	public class FieldCell : IFieldCell
	{
		private IFieldCellView _cellView;
		private IUnit _unit;
		private Vector2Int _position;

		#region IFieldCell

		public bool HasUnit => _unit != null;

		public IUnit Unit => _unit;

		public Vector2Int Position => _position;

		public FieldCell(IFieldCellView cellView, Vector2Int position)
		{
			_cellView = cellView;
			_position = position;
		}

		public void SetUnit(IUnit unit)
		{
			//_unit = unit;
			//_unit.SetViewParent(_cellView.UnitPivot);
		}

		public void Clear()
		{
			_unit = null;
		}

		#endregion
	}
}
