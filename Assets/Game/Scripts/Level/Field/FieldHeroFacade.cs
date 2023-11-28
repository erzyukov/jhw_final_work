namespace Game.Field
{
	using UniRx;
	using Zenject;

	public interface IFieldHeroFacade : IFieldFacade
	{
		IntReactiveProperty AliveUnitsCount { get; }

		void SetDraggableActive(bool value);
	}

	public class FieldHeroFacade : FieldFacade, IFieldHeroFacade
	{
		[Inject] IFieldUnits _fieldUnits;

		#region IFieldHeroFacade
		
		public IntReactiveProperty AliveUnitsCount => _fieldUnits.AliveUnitsCount;

		public void SetDraggableActive(bool value) => _fieldUnits.SetDraggableActive(value);

		#endregion
	}
}