namespace Game.Level
{
	using Utilities;
	using Units;
	using UnityEngine;
	using UniRx;
	using System;

	public interface IPlatoonCell
	{
		bool HasUnit { get; }
		void SetUnit(IUnit unit);
		void Clear();
	}

	public class PlatoonCell : ControllerBase, IPlatoonCell
	{
		private IPlatoonCellView _cellView;
		private IUnit _unit;

		readonly CompositeDisposable unitDisposable = new CompositeDisposable();

		public PlatoonCell(IPlatoonCellView cellView, Camera camera)
		{
			cellView.Init(camera);
			_cellView = cellView;
		}

		public bool HasUnit => _unit != null;

		public void SetUnit(IUnit unit)
		{
			_unit = unit;
			_unit.SetViewParent(_cellView.UnitPivot);
			SubscribeUnit();
		}

		public void Clear()
		{
			UnsubscribeUnit();
			_unit = null;
		}

		private void SubscribeUnit()
		{
			_unit.Focused
				.Subscribe(_ => OnUnitFocused())
				.AddTo(this)
				.AddTo(unitDisposable);

			_unit.Blured
				.Subscribe(_ => OnUnitBlured())
				.AddTo(this)
				.AddTo(unitDisposable);
		}

		private void UnsubscribeUnit() =>
			unitDisposable.Dispose();

		private void OnUnitFocused()
		{
			_cellView.Select();
		}

		private void OnUnitBlured()
		{
			_cellView.Deselect();
		}
	}
}