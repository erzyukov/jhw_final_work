namespace Game.Field
{
	using Game.Units;
	using Game.Utilities;
	using System;
	using System.Collections.Generic;
	using UniRx;
	using Zenject;

	public interface IFieldEvents
	{
		ReactiveCommand<IUnitFacade> UnitAdded { get; }
		ReactiveCommand<IUnitFacade> UnitRemoved { get; }
		ReactiveCommand UnitsCleared { get; }
		ReactiveCommand<IUnitFacade> UnitDragging { get; }
		ReactiveCommand<IUnitFacade> UnitDropped { get; }
	}

	public class FieldEvents : ControllerBase, IFieldEvents, IInitializable
	{
		[Inject] private IField<FieldCell> _field;

		Dictionary<IUnitFacade, IDisposable> _unitsDraggingLifetime = new Dictionary<IUnitFacade, IDisposable>();
		Dictionary<IUnitFacade, IDisposable> _unitsDroppedLifetime = new Dictionary<IUnitFacade, IDisposable>();

		public void Initialize()
		{
			Observable.Merge(
					_field.Units.ObserveAdd().Select(element => element.Value),
					_field.Units.ObserveReplace().Select(element => element.NewValue)
				)
				.Subscribe(unit => OnUnitAddedHandler(unit))
				.AddTo(this);

			Observable.Merge(
					_field.Units.ObserveRemove().Select(element => element.Value),
					_field.Units.ObserveReplace().Select(element => element.OldValue)
				)
				.Subscribe(unit => OnUnitRemovedHandler(unit))
				.AddTo(this);

			_field.Units.ObserveReset()
				.Subscribe(_ => OnUnitsRemovedHandler())
				.AddTo(this);

		}

		#region IFieldEvents

		public ReactiveCommand<IUnitFacade> UnitAdded { get; } = new ReactiveCommand<IUnitFacade>();
		public ReactiveCommand<IUnitFacade> UnitRemoved { get; } = new ReactiveCommand<IUnitFacade>();
		public ReactiveCommand UnitsCleared { get; } = new ReactiveCommand();
		public ReactiveCommand<IUnitFacade> UnitDragging { get; } = new ReactiveCommand<IUnitFacade>();
		public ReactiveCommand<IUnitFacade> UnitDropped { get; } = new ReactiveCommand<IUnitFacade>();

		#endregion

		private void OnUnitsRemovedHandler()
		{
			UnitsCleared.Execute();

			foreach (var unit in _unitsDraggingLifetime)
				unit.Value.Dispose();

			_unitsDraggingLifetime.Clear();

			foreach (var unit in _unitsDroppedLifetime)
				unit.Value.Dispose();

			_unitsDroppedLifetime.Clear();
		}

		private void OnUnitAddedHandler(IUnitFacade unit)
		{
			UnitAdded.Execute(unit);
			IDisposable dragging = unit.Dragging.Subscribe(_ => UnitDragging.Execute(unit));
			_unitsDraggingLifetime.Add(unit, dragging);
			IDisposable dropped = unit.Dropped.Subscribe(_ => UnitDropped.Execute(unit));
			_unitsDroppedLifetime.Add(unit, dropped);
		}

		private void OnUnitRemovedHandler(IUnitFacade unit)
		{
			UnitRemoved.Execute(unit);
			_unitsDraggingLifetime[unit].Dispose();
			_unitsDraggingLifetime.Remove(unit);
			_unitsDroppedLifetime[unit].Dispose();
			_unitsDroppedLifetime.Remove(unit);
		}
	}
}
