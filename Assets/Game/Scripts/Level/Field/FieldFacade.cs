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
		[Inject] private IField<FieldCell> _field;

		#region IFieldFacade

		public bool HasFreeSpace => _field.HasFreeSpace;
		public void AddUnit(IUnitFacade unit) => _field.AddUnit(unit);
		public void AddUnit(IUnitFacade unit, Vector2Int position) => _field.AddUnit(unit, position);

		#endregion
	}
}
