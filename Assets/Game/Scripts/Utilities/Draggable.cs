namespace Game.Utilities
{
	using UnityEngine;
	using UnityEngine.EventSystems;
	using Game.Core;
	using UniRx;

	public interface IDraggable
	{
		ReactiveCommand Dragging { get; }
		ReactiveCommand Dropped { get; }
		ReactiveCommand PointerDowned { get; }
		ReactiveCommand PointerUped { get; }
		void SetActive(bool value);
	}

    public class Draggable : MonoBehaviour, IDraggable, IBeginDragHandler, IDragHandler, IEndDragHandler
		, IPointerDownHandler, IPointerUpHandler
		, IInitializePotentialDragHandler
	{
		Camera _mainCamera;
		Vector3 _clickOffset = Vector3.zero;
		bool _isActive;

		void Start()
		{
			_mainCamera = Camera.main;
		}

		#region IDraggable

		public ReactiveCommand Dragging { get; } = new ReactiveCommand();
		public ReactiveCommand Dropped { get; } = new ReactiveCommand();
		public ReactiveCommand PointerDowned { get; } = new ReactiveCommand();
		public ReactiveCommand PointerUped { get; } = new ReactiveCommand();

		public void SetActive(bool value) =>
			_isActive = value;

		#endregion

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (_isActive == false)
				return;

			if (GroundRaycast(eventData.position, out Vector3 groundPosition))
			{
				_clickOffset = transform.position - groundPosition;
				Dragging.Execute();
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (_isActive == false)
				return;

			if (GroundRaycast(eventData.position, out Vector3 groundPosition))
				transform.position = groundPosition + _clickOffset;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (_isActive == false)
				return;

			Dropped.Execute();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			PointerDowned.Execute();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			PointerUped.Execute();
		}

		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			eventData.useDragThreshold = false;
		}

		private bool GroundRaycast(Vector2 mousePosition, out Vector3 groundPosition)
		{
			int groundLayerMask = 1 << Constans.GroundLayer;
			Ray ray = _mainCamera.ScreenPointToRay(mousePosition);
			groundPosition = default;

			if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
			{
				groundPosition = hit.point;
				return true;
			}

			return false;
		}
	}
}
