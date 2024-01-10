namespace Game.Field
{
	using Game.Units;
	using Game.Utilities;
	using System;
	using UniRx;
	using Zenject;

	public interface IFieldUnits
	{
		IntReactiveProperty AliveUnitsCount { get; }
		void SetDraggableActive(bool value);
		void ResetAlives();
	}

	public class FieldUnits : ControllerBase, IFieldUnits, IInitializable
	{
		[Inject] protected IField<FieldCell> Field;
		[Inject] private IFieldEvents _events;

		private bool _isDraggableActive = true;

		public void Initialize()
		{
			Observable.Merge(
					_events.UnitDying,
					_events.UnitRemoved.Where(unit => unit.IsDead == false)
				)
				.Subscribe(_ => AliveUnitsCount.Value -= 1)
				.AddTo(this);

			_events.UnitAdded
				.Subscribe(OnUnitAddedHandler)
				.AddTo(this);

			_events.UnitsCleared
				.Subscribe(_ => AliveUnitsCount.Value = 0)
				.AddTo(this);
		}

		#region IFieldUnits

		public IntReactiveProperty AliveUnitsCount { get; } = new IntReactiveProperty();

		public void SetDraggableActive(bool value)
		{
			_isDraggableActive = value;

			foreach (var unit in Field.Units)
				unit.SetDraggableActive(value);
		}

		public void ResetAlives()
		{
			AliveUnitsCount.Value = Field.Units.Count;
		}

		#endregion

		private void OnUnitAddedHandler(IUnitFacade facade)
		{
			AliveUnitsCount.Value += 1;
			facade.SetDraggableActive(_isDraggableActive);
		}
	}
}