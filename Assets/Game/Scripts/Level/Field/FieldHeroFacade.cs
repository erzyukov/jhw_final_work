namespace Game.Field
{
	using Game.Units;
	using UniRx;
	using Zenject;

	public interface IFieldHeroFacade : IFieldFacade
	{
		IntReactiveProperty AliveUnitsCount { get; }
		ReactiveCommand<IUnitFacade> UnitsMerged { get; }
		void SetDraggableActive(bool value);
		void ResetAlives();
	}

	public class FieldHeroFacade : FieldFacade, IFieldHeroFacade
	{
		[Inject] IFieldUnits _fieldUnits;

		#region IFieldHeroFacade

		public IntReactiveProperty AliveUnitsCount => _fieldUnits.AliveUnitsCount;

		public ReactiveCommand<IUnitFacade> UnitsMerged { get; } = new ReactiveCommand<IUnitFacade>();

		public void SetDraggableActive(bool value) => _fieldUnits.SetDraggableActive(value);

		public void ResetAlives() => _fieldUnits.ResetAlives();

		#endregion
	}
}