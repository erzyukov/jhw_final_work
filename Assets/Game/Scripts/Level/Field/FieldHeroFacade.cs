namespace Game.Field
{
	using UniRx;
	using Zenject;

	public interface IFieldHeroFacade : IFieldFacade
	{
		IntReactiveProperty AliveUnitsCount { get; }
		ReactiveCommand UnitsMerged { get; }
		void SetDraggableActive(bool value);
		void ResetAlives();
	}

	public class FieldHeroFacade : FieldFacade, IFieldHeroFacade
	{
		[Inject] IFieldUnits _fieldUnits;

		#region IFieldHeroFacade

		public IntReactiveProperty AliveUnitsCount => _fieldUnits.AliveUnitsCount;

		public ReactiveCommand UnitsMerged { get; } = new ReactiveCommand();

		public void SetDraggableActive(bool value) => _fieldUnits.SetDraggableActive(value);

		public void ResetAlives() => _fieldUnits.ResetAlives();

		#endregion
	}
}