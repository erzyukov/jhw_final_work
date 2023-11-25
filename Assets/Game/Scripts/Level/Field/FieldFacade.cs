namespace Game.Field
{
	using Game.Units;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public interface IFieldFacade
	{
		ReactiveCollection<IUnitFacade> Units { get; }
		bool HasFreeSpace { get; }
		bool HasUnit(IUnitFacade unit);
		Vector2Int AddUnit(IUnitFacade unit);
		Vector2Int AddUnit(IUnitFacade unit, Vector2Int position);
		void RemoveUnit(IUnitFacade unit);
		void Clear();
		void SetFieldRenderEnabled(bool value);
	}

	public class FieldFacade : MonoBehaviour, IFieldFacade
	{
		[Inject] protected IField<FieldCell> Field;
		[Inject] private IFieldView _fieldView;

		#region IFieldFacade

		public ReactiveCollection<IUnitFacade> Units => Field.Units;
		public bool HasFreeSpace => Field.HasFreeSpace;
		public bool HasUnit(IUnitFacade unit) => Field.HasUnit(unit);
		public Vector2Int AddUnit(IUnitFacade unit) => Field.AddUnit(unit);
		public Vector2Int AddUnit(IUnitFacade unit, Vector2Int position) => Field.AddUnit(unit, position);
		public void RemoveUnit(IUnitFacade unit) => Field.RemoveUnit(unit);
		public void Clear() => Field.Clear();
		public void SetFieldRenderEnabled(bool value) => Field.SetFieldRenderEnabled(value);

		#endregion
	}
}
