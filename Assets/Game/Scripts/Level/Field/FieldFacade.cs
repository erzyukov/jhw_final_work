namespace Game.Field
{
	using Game.Units;
	using UnityEngine;
	using Zenject;

	public interface IFieldFacade
	{
		bool HasFreeSpace { get; }
		void AddUnit(IUnitFacade unit);
		void AddUnit(IUnitFacade unit, Vector2Int position);
	}

	public class FieldFacade : MonoBehaviour, IFieldFacade
	{
		[Inject] protected IField<FieldCell> Field;

		#region IFieldFacade

		public bool HasFreeSpace => Field.HasFreeSpace;
		public void AddUnit(IUnitFacade unit) => Field.AddUnit(unit);
		public void AddUnit(IUnitFacade unit, Vector2Int position) => Field.AddUnit(unit, position);

		#endregion
	}
}
