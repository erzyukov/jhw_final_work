namespace Game.Units
{
	using UniRx;
	using UnityEngine;
	using UnityEngine.EventSystems;

	public interface IUnitView
	{
		ReactiveCommand PointerUped { get; }
		ReactiveCommand PointerDowned { get; }
		ReactiveCommand PointerEntered { get; }
		ReactiveCommand PointerExited { get; }
		void SetParent(Transform transform);
	}

	public class UnitView : MonoBehaviour, IUnitView, IPointerEnterHandler, IPointerDownHandler
		, IPointerUpHandler, IPointerMoveHandler, IPointerExitHandler
	{
		#region IUnitView

		public ReactiveCommand PointerUped { get; } = new ReactiveCommand();
		public ReactiveCommand PointerDowned { get; } = new ReactiveCommand();
		public ReactiveCommand PointerEntered { get; } = new ReactiveCommand();
		public ReactiveCommand PointerExited { get; } = new ReactiveCommand();

		public void SetParent(Transform transform) =>
			this.transform.SetParent(transform, false);

		#endregion

		#region PointerHandels

		public void OnPointerMove(PointerEventData eventData)
		{
			//Debug.LogWarning("MouseMove");
		}

		public void OnPointerUp(PointerEventData eventData) => 
			PointerUped.Execute();

		public void OnPointerDown(PointerEventData eventData) => 
			PointerDowned.Execute();

		public void OnPointerEnter(PointerEventData eventData) => 
			PointerEntered.Execute();

		public void OnPointerExit(PointerEventData eventData) =>
			PointerExited.Execute();

		#endregion
	}
}
