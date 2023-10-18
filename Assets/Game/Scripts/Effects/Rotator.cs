namespace Game.Effects
{
	using UnityEngine;
	using DG.Tweening;

    public class Rotator : MonoBehaviour
    {
		[SerializeField] private Vector3 _direction;
		[SerializeField] private float _rotatePerMinutes;

		private Tween _tween;

		private void Start()
		{
			float secondsInMinute = 60;
			float duration = secondsInMinute / _rotatePerMinutes;
			float fullAngle = 360;
			_tween = transform.DOLocalRotate(_direction * fullAngle, duration, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
		}

		private void OnDestroy()
		{
			_tween.Kill();
		}
	}
}
