namespace Game
{
	using UnityEngine;
	using UnityEngine.UI;

	public interface IPlatoonCellView
	{
		Transform UnitPivot { get; }
		void Init(Camera camera);
		void SetPosition(Vector3 position);
		void Select();
		void Deselect();
	}

	public class PlatoonCellView : MonoBehaviour, IPlatoonCellView
	{
		[SerializeField] private Transform _unitPivot;
		[SerializeField] private Canvas _canvas;
		[SerializeField] private Image _platform;
		[SerializeField] private Color _selectedColor;


		private Color _defaultColor;

		public Transform UnitPivot => _unitPivot;

		private void Start()
		{
			_defaultColor = _platform.color;
		}

		public void Init(Camera camera)
		{
			_canvas.worldCamera = camera;
		}

		public void SetPosition(Vector3 position) => transform.position = position;

		public void Select()
		{
			_platform.color = _selectedColor;
		}

		public void Deselect()
		{
			_platform.color = _defaultColor;
		}
	}
}