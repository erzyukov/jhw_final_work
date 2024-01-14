namespace Game.Ui
{
	using UnityEngine;

	[ExecuteInEditMode]
	public class Billboard : MonoBehaviour
	{
		[SerializeField] Canvas _canvas;

		Camera _mainCamera;

		private void Start()
		{
			_mainCamera = Camera.main;
			if (_canvas != null )
				_canvas.worldCamera = _mainCamera;
		}

		private void LateUpdate()
		{
			if (_mainCamera != null)
				transform.LookAt(
					transform.position + _mainCamera.transform.forward,
					_mainCamera.transform.up
				);
		}
	}
}

