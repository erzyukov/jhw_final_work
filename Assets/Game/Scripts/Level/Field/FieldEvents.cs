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
		ReactiveCommand<IUnitFacade> UnitPointerDowned { get; }
		ReactiveCommand<IUnitFacade> UnitPointerUped { get; }
		ReactiveCommand<IUnitFacade> UnitMergeInitiated { get; }
		ReactiveCommand<IUnitFacade> UnitMergeCanceled { get; }
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

		public ReactiveCommand<IUnitFacade> UnitAdded { get; } = new ReactiveCommand<IUnitFacade>();
		public ReactiveCommand<IUnitFacade> UnitRemoved { get; } = new ReactiveCommand<IUnitFacade>();
		public ReactiveCommand UnitsCleared { get; } = new ReactiveCommand();
		public ReactiveCommand<IUnitFacade> UnitDragging { get; } = new ReactiveCommand<IUnitFacade>();
		public ReactiveCommand<IUnitFacade> UnitDropped { get; } = new ReactiveCommand<IUnitFacade>();
		public ReactiveCommand<IUnitFacade> UnitPointerDowned { get; } = new ReactiveCommand<IUnitFacade>();
		public ReactiveCommand<IUnitFacade> UnitPointerUped { get; } = new ReactiveCommand<IUnitFacade>();
		public ReactiveCommand<IUnitFacade> UnitMergeInitiated { get; } = new ReactiveCommand<IUnitFacade>();
		public ReactiveCommand<IUnitFacade> UnitMergeCanceled { get; } = new ReactiveCommand<IUnitFacade>();
		public ReactiveCommand<IUnitFacade> UnitDied { get; } = new ReactiveCommand<IUnitFacade>();

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

			unit.Dragging.Subscribe(_ => UnitDragging.Execute(unit)).AddTo(disposable);
			unit.Dropped.Subscribe(_ => UnitDropped.Execute(unit)).AddTo(disposable);
			unit.PointerDowned.Subscribe(_ => UnitPointerDowned.Execute(unit)).AddTo(disposable);
			unit.PointerUped.Subscribe(_ => UnitPointerUped.Execute(unit)).AddTo(disposable);
			unit.MergeInitiated.Subscribe(_ => UnitMergeInitiated.Execute(unit)).AddTo(disposable);
			unit.MergeCanceled.Subscribe(_ => UnitMergeCanceled.Execute(unit)).AddTo(disposable);
			unit.Dying.Subscribe(_ => UnitDied.Execute(unit)).AddTo(disposable);
		}

		private void OnUnitRemovedHandler(IUnitFacade unit)
		{
			UnitRemoved.Execute(unit);
			_unitsLifetime[unit].Dispose();
			_unitsLifetime.Remove(unit);
		}
	}
}
