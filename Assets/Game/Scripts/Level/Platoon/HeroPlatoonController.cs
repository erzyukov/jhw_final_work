namespace Game.Platoon
{
	using Utilities;
	using Units;
	using Unit = Units.Unit;
	using Ui;
	using VContainer;
	using VContainer.Unity;
	using UniRx;
	using System.Collections.Generic;
	using System;
	using static UnityEngine.UI.CanvasScaler;
	using UnityEngine;

	public class HeroPlatoonController : ControllerBase, IStartable
	{
		[Inject] private IPlatoon _platoon;
		[Inject] private IHudUnitPanel _hudUnitPanel;
		[Inject] private UnitViewFactory _unitViewFactory;

		const float UnitRaiseYOffset = 0.3f;
		const float UnitDefaultYOffset = 0;

		private Dictionary<IUnit, CompositeDisposable> _unitDisposable = new Dictionary<IUnit, CompositeDisposable>();
		private State _state;
		private IHeroUnit _risedUnit;
		private IPlatoonCell _lastSelectedCell;

		public void Start()
		{
			_hudUnitPanel.UnitSelectButtonPressed
				.Subscribe(CreateUnit)
				.AddTo(this);

			/*
			_platoon.UnitRemoved
				.Subscribe(OnUnitRemoved)
				.AddTo(this);
			*/

			_platoon.PointerEnteredToCell
				.Subscribe(OnPointerEnteredToCell)
				.AddTo(this);
		}

		private void CreateUnit(Unit.Kind type)
		{
			if (_platoon.HasFreeSpace)
			{
				IUnitView view = _unitViewFactory.Create(type);
				IHeroUnit unit = new HeroUnit(type, view);
				_platoon.AddUnit(unit);

				SubscribeUnit(unit);
			}
		}

		private void SubscribeUnit(IHeroUnit unit)
		{
			_unitDisposable.Add(unit, new CompositeDisposable());
			//PlatoonCell platoonCell = _platoon.GetPlatoonCell(unit.Position);

			unit.Rised
				.Subscribe(_ => OnUnitRised(unit))
				.AddTo(this)
				.AddTo(_unitDisposable[unit]);

			unit.PutDowned
				.Subscribe(_ => OnUnitPutDowned(unit))
				.AddTo(this)
				.AddTo(_unitDisposable[unit]);

			unit.Focused
				.Where(_ => _state != State.UnitRised)
				.Subscribe(_ => SelectCell(_platoon.GetPlatoonCell(unit.Position)))
				.AddTo(this)
				.AddTo(_unitDisposable[unit]);

			unit.Blured
				.Subscribe(_ => _platoon.GetPlatoonCell(unit.Position).DeselectCell())
				.AddTo(this)
				.AddTo(_unitDisposable[unit]);
		}

/*
		private void OnUnitRemoved(IUnit unit)
		{
			_unitDisposable[unit].Dispose();
			_unitDisposable.Remove(unit);
		}
*/
		private void OnUnitRised(IHeroUnit unit)
		{
			unit.Transform.localPosition = unit.Transform.localPosition.WithY(UnitRaiseYOffset);
			_state = State.UnitRised;
			_risedUnit = unit;
		}

		private void OnUnitPutDowned(IHeroUnit unit)
		{
			unit.Transform.localPosition = unit.Transform.localPosition.WithY(UnitDefaultYOffset);
			_state = State.None;
			_risedUnit = null;
		}

		private void OnPointerEnteredToCell(PlatoonCell cell)
		{
			if (_state != State.UnitRised)
				return;

			SelectCell(cell);

			if (cell.HasUnit)
				return;

			_platoon.RemoveUnit(_risedUnit);
			_platoon.AddUnit(_risedUnit, cell);

			_risedUnit.Transform.localPosition = _risedUnit.Transform.localPosition.WithY(UnitRaiseYOffset);
		}

		private void SelectCell(IPlatoonCell cell)
		{
			_lastSelectedCell?.DeselectCell();
			_lastSelectedCell = cell;
			_lastSelectedCell.SelectCell();
		}

		private enum State
		{
			None,
			UnitFocused,
			UnitRised,
		}
	}
}