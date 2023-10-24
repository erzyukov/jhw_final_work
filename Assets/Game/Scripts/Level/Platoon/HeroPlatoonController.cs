namespace Game.Platoon
{
	using Utilities;
	using Configs;
	using Units;
	using Unit = Units.Unit;
	using Ui;
	using VContainer;
	using VContainer.Unity;
	using UniRx;
	using System.Collections.Generic;

	public class HeroPlatoonController : ControllerBase, IStartable
	{
		[Inject] private IPlatoon _platoon;
		[Inject] private IHudUnitPanel _hudUnitPanel;
		[Inject] private UnitViewFactory _unitViewFactory;
		[Inject] private UnitsConfig _unitsConfig;

		const float UnitRaiseYOffset = 0.3f;
		const float UnitDefaultYOffset = 0;

		private Dictionary<IUnit, CompositeDisposable> _unitDisposable = new Dictionary<IUnit, CompositeDisposable>();
		private State _state;
		private IHeroUnit _risedUnit;
		private IUnit _swapedUnit;
		private IPlatoonCell _lastSelectedCell;
		private IPlatoonCell _initialUnitCell;

		public void Start()
		{
			_hudUnitPanel.UnitSelectButtonPressed
				.Subscribe(CreateUnit)
				.AddTo(this);

			_platoon.PointerEnteredCell
				.Subscribe(OnPointerEnteredCell)
				.AddTo(this);

			_platoon.PointerExitedCell
				.Subscribe(OnPointerExitedCell)
				.AddTo(this);
		}

		private void CreateUnit(Unit.Kind type)
		{
			if (_platoon.HasFreeSpace)
			{
				IUnitView view = _unitViewFactory.Create(type);
				IHeroUnit unit = new HeroUnit(type, view, _unitsConfig.Units[type]);
				_platoon.AddUnit(unit);

				SubscribeUnit(unit);
			}
		}

		private void SubscribeUnit(IHeroUnit unit)
		{
			_unitDisposable.Add(unit, new CompositeDisposable());

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
				.Subscribe(_ => OnCellFocused(_platoon.GetCell(unit)))
				.AddTo(this)
				.AddTo(_unitDisposable[unit]);

			unit.Blured
				.Subscribe(_ => _platoon.GetCell(unit).DeselectCell())
				.AddTo(this)
				.AddTo(_unitDisposable[unit]);
		}

		private void OnUnitRised(IHeroUnit unit)
		{
			_platoon.SetIgnoreUnistRaycast(true);
			_initialUnitCell = _platoon.GetCell(unit);
			ApplyUnitRisedFx(unit);
			_state = State.UnitRised;
			_risedUnit = unit;
		}

		private void OnUnitPutDowned(IHeroUnit unit)
		{
			_platoon.SetIgnoreUnistRaycast(false);
			RemoveUnitRisedFx(unit);
			_state = State.None;
			_risedUnit = null;
			_initialUnitCell = null;
			_swapedUnit = null;
		}

		private void ApplyUnitRisedFx(IHeroUnit unit) =>
			unit.Transform.localPosition = unit.Transform.localPosition.WithY(UnitRaiseYOffset);

		private void RemoveUnitRisedFx(IHeroUnit unit) =>
			unit.Transform.localPosition = unit.Transform.localPosition.WithY(UnitDefaultYOffset);

		private void OnPointerEnteredCell(PlatoonCell cell)
		{
			if (_state != State.UnitRised)
				return;

			OnCellFocused(cell);

			if (cell.HasUnit && cell.Unit != _risedUnit)
				SwapRisedUnitWithCell(cell);
			else
				MoveRisedUnitToCell(cell);

			ApplyUnitRisedFx(_risedUnit);
		}

		private void SwapRisedUnitWithCell(PlatoonCell cell)
		{
			_swapedUnit = cell.Unit;

			_platoon.RemoveUnit(_risedUnit);
			_platoon.RemoveUnit(_swapedUnit);

			_platoon.AddUnit(_risedUnit, cell);
			_platoon.AddUnit(_swapedUnit, _initialUnitCell);
		}

		private void MoveRisedUnitToCell(PlatoonCell cell)
		{
			_platoon.RemoveUnit(_risedUnit);
			_platoon.AddUnit(_risedUnit, cell);
		}

		private void OnPointerExitedCell(PlatoonCell cell)
		{
			if (_state != State.UnitRised || _swapedUnit == null)
				return;
			
			_platoon.RemoveUnit(_risedUnit);
			_platoon.RemoveUnit(_swapedUnit);

			_platoon.AddUnit(_swapedUnit, cell);

			_swapedUnit = null;
		}

		private void OnCellFocused(IPlatoonCell cell)
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
			UnitMoved,
		}
	}
}