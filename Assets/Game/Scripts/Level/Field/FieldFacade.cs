namespace Game.Field
{
	using Game.Units;
	using System.Collections.Generic;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public interface IFieldFacade
	{
		ReactiveCollection<IUnitFacade> Units { get; }
		bool HasFreeSpace { get; }
		IFieldEvents Events { get; }
		bool HasUnit(IUnitFacade unit);
		Vector2Int AddUnit(IUnitFacade unit);
		Vector2Int AddUnit(IUnitFacade unit, Vector2Int position);
		IFieldCell GetCell(IUnitFacade unit);
		void RemoveUnit(IUnitFacade unit);
		void Clear();
		void SetFieldRenderEnabled(bool value);
		List<IFieldCell> FindSameUnitCells(IUnitFacade unit);
	}

	public class FieldFacade : MonoBehaviour, IFieldFacade
	{
		[Inject] protected IField<FieldCell> Field;
		[Inject] private IFieldEvents _events;

		#region IFieldFacade

		public ReactiveCollection<IUnitFacade> Units => Field.Units;
		public bool HasFreeSpace => Field.HasFreeSpace;
		public IFieldEvents Events => _events;
		public bool HasUnit(IUnitFacade unit) => Field.HasUnit(unit);
		public virtual Vector2Int AddUnit(IUnitFacade unit) => Field.AddUnit(unit);
		public virtual Vector2Int AddUnit(IUnitFacade unit, Vector2Int position) => Field.AddUnit(unit, position);
		public IFieldCell GetCell(IUnitFacade unit) => Field.GetCell(unit);
		public void RemoveUnit(IUnitFacade unit) => Field.RemoveUnit(unit);
		public virtual void Clear() => Field.Clear();
		public void SetFieldRenderEnabled(bool value) => Field.SetFieldRenderEnabled(value);
		public List<IFieldCell> FindSameUnitCells(IUnitFacade unit) => Field.FindSameUnitCells(unit);

		#endregion
	}
}
