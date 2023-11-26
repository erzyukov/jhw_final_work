namespace Game.Field
{
	using UnityEngine;

	public interface IFieldCellView
	{
		Transform UnitPivot { get; }
		void SetPosition(Vector3 position);
		void SetColor(Color color);
		void SetSelectedColor(Color color);
		void SetFieldCellRenderEnabled(bool value);
		void Select();
		void Deselect();
	}

	public class FieldCellView : MonoBehaviour, IFieldCellView
	{
		[SerializeField] private Transform _unitPivot;
		[SerializeField] private Renderer _renderer;

		private Color _color;
		private Color _selectedColor;

		public Transform UnitPivot => _unitPivot;

		public void SetPosition(Vector3 position) => 
			transform.position = position;

		public void SetColor(Color color)
		{
			_color = color;
			_renderer.material.SetColor("_Color", color);
		}

		public void SetSelectedColor(Color color) =>
			_selectedColor = color;

		public void SetFieldCellRenderEnabled(bool value) =>
			_renderer.enabled = value;

		public void Select() =>
			_renderer.material.SetColor("_Color", _selectedColor);

		public void Deselect() =>
			_renderer.material.SetColor("_Color", _color);
	}
}