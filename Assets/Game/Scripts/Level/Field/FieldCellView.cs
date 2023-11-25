namespace Game.Field
{
	using UnityEngine;

	public interface IFieldCellView
	{
		Transform UnitPivot { get; }
		void SetPosition(Vector3 position);
		void SetColor(Color color);
		void SetFieldCellRenderEnabled(bool value);
	}

	public class FieldCellView : MonoBehaviour, IFieldCellView
	{
		[SerializeField] private Transform _unitPivot;
		[SerializeField] private Renderer _renderer;

		public Transform UnitPivot => _unitPivot;

		public void SetPosition(Vector3 position) => 
			transform.position = position;

		public void SetColor(Color color) =>
			_renderer.material.SetColor("_Color", color);

		public void SetFieldCellRenderEnabled(bool value) =>
			_renderer.enabled = value;
	}
}