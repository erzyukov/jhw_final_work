namespace Game.Ui
{
	using UnityEngine;
	using DG.Tweening;
	using UnityEngine.UI;
	using System;

	public interface IUiVeil
	{
		void Fade(Action onCompete = null);
		void Appear(Action onCompete = null);
		void SetActive(bool value);
	}

    public class UiVeil : MonoBehaviour, IUiVeil
	{
		[SerializeField] private Image _veil;
		[SerializeField] private float _duration;

		#region IUiViel

		public void Fade(Action onCompete = null)
		{
			if (gameObject.activeSelf == false)
			{
				onCompete?.Invoke();
			}
			else
			{
				AnimateVeilAlpha(0, () =>
				{
					SetActive(false);
					onCompete?.Invoke();
				});
			}
		}
		
		public void Appear(Action onCompete = null)
		{
			if (gameObject.activeSelf == true)
			{
				onCompete?.Invoke();
			}
			else
			{
				SetActive(true);
				AnimateVeilAlpha(1, () => onCompete?.Invoke());
			}
		}

		public void SetActive(bool value) => gameObject.SetActive(value);

		#endregion

		void AnimateVeilAlpha(float endValue, Action onCompete)
		{
			_veil.DOFade(endValue, _duration)
				.OnComplete(() => onCompete.Invoke());
		}
	}
}