namespace Game.Tutorial
{
	using UnityEngine;
	using DG.Tweening;

	public interface IFingerSlideHint
	{
		void SetPositions(Vector3 from, Vector3 to);
		void PlayAnimation();
		void SetActive(bool value);
	}

	public class FingerSlideHint : MonoBehaviour, IFingerSlideHint
	{
		[SerializeField] float _yOffset;
		[SerializeField] float _duration;
		[SerializeField] Ease _ease;

		private Vector3 _target;
		Camera _camera;
		Tween _tween;

		private void OnEnable()
		{
			_camera = Camera.main;
		}

		#region IFingerHint

		public void SetActive(bool value)
		{
			if (value == false)
			{
				_tween.Rewind();
				_tween.Kill();
			}

			gameObject.SetActive(value);
		}

		public void SetPositions(Vector3 from, Vector3 to)
		{
			transform.position = _camera.WorldToScreenPoint(from + Vector3.up * _yOffset);
			_target = _camera.WorldToScreenPoint(to + Vector3.up * _yOffset);
		}

		public void PlayAnimation()
		{
			_tween = transform.DOMove(_target, _duration).SetLoops(-1, LoopType.Restart).SetEase(_ease);
		}

		#endregion
	}
}
