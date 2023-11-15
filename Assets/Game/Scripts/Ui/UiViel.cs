namespace Game.Ui
{
	using UnityEngine;
	using DG.Tweening;
	using UnityEngine.UI;
	using System;

	public interface IUiViel
	{
		void SetActive(bool value, Action onCompete = null);
	}

    public class UiViel : MonoBehaviour, IUiViel
	{
		[SerializeField] private Image _veil;
		[SerializeField] private float _duration;

		public void SetActive(bool value, Action onCompete = null)
		{
			if (value)
				gameObject.SetActive(true);

			float alpha = value ? 1 : 0;
			_veil.DOFade(alpha, _duration)
				.OnComplete(() =>
				{
					onCompete?.Invoke();

					if (value == false)
						gameObject.SetActive(false);
				});
		}
	}
}