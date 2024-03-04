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
		ReactiveCommand<IUnitFacade> UnitDrag { get; }
		ReactiveCommand<IUnitFacade> UnitDropped { get; }
		ReactiveCommand<IUnitFacade> UnitPointerDowned { get; }
		ReactiveCommand<IUnitFacade> UnitPointerUped { get; }
		ReactiveCommand<IUnitFacade> UnitAttacking { get; }
		ReactiveCommand<IUnitFacade> UnitDying { get; }
		ReactiveCommand<IUnitFacade> UnitDied { get; }
	}

	public class FieldEvents : ControllerBase, IFieldEvents, IInitializable
	{
		[Inject] private IField<FieldCell> _field;

		Dictionary<IUnitFacade, IDisposable> _unitsLifetime = new Dictionary<IUnitFacade, IDisposable>();

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

		public ReactiveCommand<IUnitFacade> UnitAdded { get; } = new();
		public ReactiveCommand<IUnitFacade> UnitRemoved { get; } = new();
		public ReactiveCommand UnitsCleared { get; } = new();
		public ReactiveCommand<IUnitFacade> UnitDragging { get; } = new();
		public ReactiveCommand<IUnitFacade> UnitDrag { get; } = new();
		public ReactiveCommand<IUnitFacade> UnitDropped { get; } = new();
		public ReactiveCommand<IUnitFacade> UnitPointerDowned { get; } = new();
		public ReactiveCommand<IUnitFacade> UnitPointerUped { get; } = new();
		public ReactiveCommand<IUnitFacade> UnitAttacking { get; } = new();
		public ReactiveCommand<IUnitFacade> UnitDying { get; } = new();
		public ReactiveCommand<IUnitFacade> UnitDied { get; } = new();

		#endregion

		private void OnUnitsRemovedHandler()
		{
			UnitsCleared.Execute();

			foreach (var unit in _unitsLifetime)
				unit.Value.Dispose();

			_unitsLifetime.Clear();
		}

		private void OnUnitAddedHandler(IUnitFacade unit)
		{
			UnitAdded.Execute(unit);

			CompositeDisposable disposable = new CompositeDisposable();
			_unitsLifetime.Add(unit, disposable);

			unit.Events.Dragging.Subscribe(_ => UnitDragging.Execute(unit)).AddTo(disposable);
			unit.Events.Drag.Subscribe(_ => UnitDrag.Execute(unit)).AddTo(disposable);
			unit.Events.Dropped.Subscribe(_ => UnitDropped.Execute(unit)).AddTo(disposable);
			unit.Events.PointerDowned.Subscribe(_ => UnitPointerDowned.Execute(unit)).AddTo(disposable);
			unit.Events.PointerUped.Subscribe(_ => UnitPointerUped.Execute(unit)).AddTo(disposable);
			//unit.Events.MergeInitiated.Subscribe(_ => UnitMergeInitiated.Execute(unit)).AddTo(disposable);
			//unit.Events.MergeCanceled.Subscribe(_ => UnitMergeCanceled.Execute(unit)).AddTo(disposable);
			unit.Events.Attacking.Subscribe(_ => UnitAttacking.Execute(unit)).AddTo(disposable);
			unit.Events.Dying.Subscribe(_ => UnitDying.Execute(unit)).AddTo(disposable);
			unit.Events.Died.Subscribe(_ => UnitDied.Execute(unit)).AddTo(disposable);
		}

		private void OnUnitRemovedHandler(IUnitFacade unit)
		{
			UnitRemoved.Execute(unit);
			_unitsLifetime[unit].Dispose();
			_unitsLifetime.Remove(unit);
		}
	}
}
