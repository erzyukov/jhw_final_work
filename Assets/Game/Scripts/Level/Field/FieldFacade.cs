namespace Game.Field
{
	using Game.Units;
	using System.Collections.Generic;
	using UnityEngine;
	using Zenject;

	public interface IFieldFacade
	{
		List<IUnitFacade> Units { get; }
		bool HasFreeSpace { get; }
		bool HasUnit(IUnitFacade unit);
		void AddUnit(IUnitFacade unit);
		void AddUnit(IUnitFacade unit, Vector2Int position);
		void RemoveUnit(IUnitFacade unit);
	}

	public class FieldFacade : MonoBehaviour, IFieldFacade
	{
		[Inject] protected IField<FieldCell> Field;

		#region IFieldFacade

		public List<IUnitFacade> Units => Field.Units;
		public bool HasFreeSpace => Field.HasFreeSpace;
		public bool HasUnit(IUnitFacade unit) => Field.HasUnit(unit);
		public void AddUnit(IUnitFacade unit) => Field.AddUnit(unit);
		public void AddUnit(IUnitFacade unit, Vector2Int position) => Field.AddUnit(unit, position);
		public void RemoveUnit(IUnitFacade unit) => Field.RemoveUnit(unit);

		#endregion
	}
}
