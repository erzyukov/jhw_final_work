namespace Game.Field
{
	using UnityEngine;

	public interface IFieldCellView
	{
		Transform UnitPivot { get; }
		void SetPosition(Vector3 position);
	}

	public class FieldCellView : MonoBehaviour, IFieldCellView
	{
		[SerializeField] private Transform _unitPivot;

		public Transform UnitPivot => _unitPivot;

		public void SetPosition(Vector3 position) => transform.position = position;
	}
}