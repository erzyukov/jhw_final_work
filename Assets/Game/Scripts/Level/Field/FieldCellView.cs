namespace Game.Field
{
	using Newtonsoft.Json.Linq;
	using UnityEngine;

	public interface IFieldCellView
	{
		Transform UnitPivot { get; }
		void SetPosition(Vector3 position);
		//void SetColor(Color color);
		void SetSprite(Sprite value);
		void SetSelectedSprite(Sprite value);
		void SetFieldCellRenderEnabled(bool value);
		void Select();
		void Deselect();
	}

	public class FieldCellView : MonoBehaviour, IFieldCellView
	{
		[SerializeField] private Transform _unitPivot;
		//[SerializeField] private Renderer _renderer;
		[SerializeField] private SpriteRenderer _spriteRenderer;

		//private Color _color;
		//private Color _selectedColor;

		private Sprite _sprite;
		private Sprite _selectedSprite;

		public Transform UnitPivot => _unitPivot;

		public void SetPosition(Vector3 position) => 
			transform.position = position;

		public void SetSprite(Sprite value)
		{
			_sprite = value;
			_spriteRenderer.sprite = value;
		}

		public void SetSelectedSprite(Sprite value) =>
			_selectedSprite = value;

		public void SetFieldCellRenderEnabled(bool value) =>
			_spriteRenderer.enabled = value;
		public void Select() =>
			_spriteRenderer.sprite = _selectedSprite;

		public void Deselect() =>
			_spriteRenderer.sprite = _sprite;
	}
}