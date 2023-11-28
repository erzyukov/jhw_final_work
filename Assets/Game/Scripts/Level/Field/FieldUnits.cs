namespace Game.Field
{
	using Game.Utilities;
	using UniRx;
	using Zenject;

	public interface IFieldUnits
	{
		IntReactiveProperty AliveUnitsCount { get; }
		void SetDraggableActive(bool value);
	}

	public class FieldUnits : ControllerBase, IFieldUnits, IInitializable
	{
		[Inject] protected IField<FieldCell> Field;
		[Inject] private IFieldEvents _events;

		public void Initialize()
		{
			Observable.Merge(
					_events.UnitDied,
					_events.UnitRemoved
				)
				.Subscribe(_ => AliveUnitsCount.Value -= 1);

			_events.UnitAdded
				.Subscribe(_ => AliveUnitsCount.Value += 1)
				.AddTo(this);

			_events.UnitsCleared
				.Subscribe(_ => AliveUnitsCount.Value = 0)
				.AddTo(this);
		}

		#region IFieldUnits

		public IntReactiveProperty AliveUnitsCount { get; } = new IntReactiveProperty();

		public void SetDraggableActive(bool value)
		{
			foreach (var unit in Field.Units)
				unit.SetDraggableActive(value);
		}

		#endregion
	}
}