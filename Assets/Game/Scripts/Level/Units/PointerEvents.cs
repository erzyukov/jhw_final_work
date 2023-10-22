namespace Game
{
	using UniRx;
	using UnityEngine;
	using UnityEngine.EventSystems;

	public interface IPointerEvents
	{
		ReactiveCommand PointerUped { get; }
		ReactiveCommand PointerDowned { get; }
		ReactiveCommand PointerEntered { get; }
		ReactiveCommand PointerExited { get; }
	}

	public class PointerEvents : MonoBehaviour, IPointerEvents
		, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
	{
		public ReactiveCommand PointerUped { get; } = new ReactiveCommand();
		public ReactiveCommand PointerDowned { get; } = new ReactiveCommand();
		public ReactiveCommand PointerEntered { get; } = new ReactiveCommand();
		public ReactiveCommand PointerExited { get; } = new ReactiveCommand();

		public void OnPointerUp(PointerEventData eventData) =>
			PointerUped.Execute();

		public void OnPointerDown(PointerEventData eventData) =>
			PointerDowned.Execute();

		public void OnPointerEnter(PointerEventData eventData) =>
			PointerEntered.Execute();

		public void OnPointerExit(PointerEventData eventData) =>
			PointerExited.Execute();
	}
}
