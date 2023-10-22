namespace Game.Platoon
{
	using Utilities;
	using Units;
	using UnityEngine;
	using UniRx;

	public interface IPlatoonCell
	{
		ReactiveCommand PointerEntred { get; }
		ReactiveCommand PointerExited { get; }
		bool HasUnit { get; }
		IUnit Unit { get; }
		Vector2Int Position { get; }
		void SetUnit(IUnit unit);
		void Clear();
		void SelectCell();
		void DeselectCell();
	}

	public class PlatoonCell : ControllerBase, IPlatoonCell
	{
		private IPlatoonCellView _cellView;
		private IUnit _unit;
		private Vector2Int _position;

		public PlatoonCell(IPlatoonCellView cellView, Camera camera, Vector2Int position)
		{
			cellView.Init(camera);
			_cellView = cellView;
			_position = position;
		}

		public ReactiveCommand PointerEntred => _cellView.PointerEntred;
		public ReactiveCommand PointerExited => _cellView.PointerExited;

		public bool HasUnit => _unit != null;

		public IUnit Unit => _unit;

		public Vector2Int Position => _position;

		public void SetUnit(IUnit unit)
		{
			_unit = unit;
			_unit.SetViewParent(_cellView.UnitPivot);
		}

		public void Clear()
		{
			_unit = null;
		}

		public void SelectCell() =>
			_cellView.Select();

		public void DeselectCell() =>
			_cellView.Deselect();
	}
}