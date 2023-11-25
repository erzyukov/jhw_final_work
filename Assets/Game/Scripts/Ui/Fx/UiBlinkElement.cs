namespace Game.Ui
{
	using DG.Tweening;
	using UnityEngine;

    public class UiBlinkElement : MonoBehaviour
    {
		[SerializeField] private float _duration;
		[SerializeField] private Ease _ease;
		[SerializeField] private CanvasGroup _canvasGroup;

        private void Start()
        {
			_canvasGroup.alpha = 0;

			_canvasGroup.DOFade(1, _duration)
				.SetLoops(-1, LoopType.Yoyo)
				.SetEase(_ease);
		}
    }
}
