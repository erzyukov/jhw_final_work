﻿namespace Game.Field
{
	using Game.Units;
	using UnityEngine;

	public interface IFieldCell
	{
		bool HasUnit { get; }
		IUnitFacade Unit { get; }
		Vector2Int Position { get; }
		void SetUnit(IUnitFacade unit);
		void Clear();
		void SetFieldCellRenderEnabled(bool value);
	}

	public class FieldCell : IFieldCell
	{
		private IFieldCellView _cellView;
		private IUnitFacade _unit;
		private Vector2Int _position;

		#region IFieldCell

		public bool HasUnit => _unit != null;

		public IUnitFacade Unit => _unit;

		public Vector2Int Position => _position;

		public FieldCell(IFieldCellView cellView, Vector2Int position)
		{
			_cellView = cellView;
			_position = position;
		}

		public void SetUnit(IUnitFacade unit)
		{
			_unit = unit;
			_unit.SetViewParent(_cellView.UnitPivot);
		}

		public void Clear()
		{
			_unit.SetViewParent(null);
			_unit = null;
		}

		public void SetFieldCellRenderEnabled(bool value) =>
			_cellView.SetFieldCellRenderEnabled(value);

		#endregion
	}
}