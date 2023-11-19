namespace Game.Field
{
	using Game.Units;
	using UnityEngine;
	using Zenject;

	public interface IFieldFacade
	{
		bool HasFreeSpace { get; }
		void AddUnit(IUnitFacade unit);
	}

	public class FieldFacade : MonoBehaviour, IFieldFacade
	{
		[Inject] private IField<FieldCell> _field;

		#region MyRegion

		public bool HasFreeSpace => _field.HasFreeSpace;
		public void AddUnit(IUnitFacade unit) => _field.AddUnit(unit);

		#endregion
	}
}
