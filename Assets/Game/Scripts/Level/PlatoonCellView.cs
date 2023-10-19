namespace Game
{
	using UnityEngine;
	using UnityEngine.UI;

	public interface IPlatoonCellView
	{
		Transform UnitPivot { get; }
		void Init(Camera camera);
		void SetPosition(Vector3 position);
	}

	public class PlatoonCellView : MonoBehaviour, IPlatoonCellView
	{
		[SerializeField] private Transform _unitPivot;
		[SerializeField] private Canvas _canvas;
		[SerializeField] private Button _button;

		public Transform UnitPivot => _unitPivot;

		public void Init(Camera camera)
		{
			_canvas.worldCamera = camera;
		}

		public void SetPosition(Vector3 position) => transform.position = position;
	}
}