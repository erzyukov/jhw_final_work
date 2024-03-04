namespace Game.Utilities
{
	using UnityEngine;
	using UnityEngine.EventSystems;
	using Game.Core;
	using UniRx;

	public interface IDraggable
	{
		ReactiveCommand<Vector2> Drag { get; }
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

		public ReactiveCommand<Vector2> Drag { get; } = new();
		public ReactiveCommand Dragging { get; } = new();
		public ReactiveCommand Dropped { get; } = new();
		public ReactiveCommand PointerDowned { get; } = new();
		public ReactiveCommand PointerUped { get; } = new();

		public void SetActive(bool value) =>
			_isActive = value;

		#endregion

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (IsActionAvailable(eventData) == false)
				return;

			if (GroundRaycast(eventData.position, out Vector3 groundPosition))
			{
				_clickOffset = transform.position - groundPosition;
				Dragging.Execute();
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (IsActionAvailable(eventData) == false)
				return;

			if (GroundRaycast( eventData.position, out Vector3 groundPosition ))
				transform.position = groundPosition + _clickOffset;

			Drag.Execute( transform.position.xz() );
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (IsActionAvailable(eventData) == false)
				return;

			Dropped.Execute();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (IsActionAvailable(eventData) == false)
				return;

			PointerDowned.Execute();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (IsActionAvailable(eventData) == false)
				return;

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

		private bool IsActionAvailable(PointerEventData eventData) =>
			_isActive
			&& eventData.button != PointerEventData.InputButton.Right
			&& eventData.button != PointerEventData.InputButton.Middle;

	}
}
