namespace Game.Ui
{
	using UnityEngine;
	using DG.Tweening;

    public class UiScaleElement : MonoBehaviour
    {
		[SerializeField] private float _minValue;
		[SerializeField] private float _duration;

		private Tween _animation;

        private void OnEnable()
        {
			transform.localScale = Vector3.one;
			_animation = transform
				.DOScale(_minValue, _duration)
				.SetLoops(-1, LoopType.Yoyo)
				.SetEase(Ease.InOutSine);
		}

		private void OnDisable()
		{
			_animation.Kill();
		}
	}
}
