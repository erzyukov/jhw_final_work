namespace Game.Ui
{
	using System;
	using UnityEngine;
	using DG.Tweening;
	using Game.Core;
	using Zenject;
	using TMPro;

	public enum UiMessage
	{
		NotEnoughSummonCurrency,
		NotEnoughFreeSpace,
		NotEnoughChestKeys,
		NotEnoughSoftCurrency,
	}

	public interface IUiMessage
	{
		void ShowMessage(UiMessage type);
	}

	public class UiTextMessage : MonoBehaviour, IUiMessage
	{
		[Inject] private ILocalizator _localizator;

		[SerializeField] private float _showDuration;
		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private TextMeshProUGUI _message;

		Sequence _sequence;

		#region IHaveNeedOfMessage

		public void ShowMessage(UiMessage type)
		{
			_message.text = _localizator.GetString(type.ToString());

			_sequence?.Kill();

            _canvasGroup.alpha = 0;
			gameObject.SetActive(true);

			float quarterDuration = _showDuration / 4;
			float halfDuration = _showDuration / 2;

			_sequence = DOTween.Sequence();
			_sequence.Append(_canvasGroup.DOFade(1, quarterDuration).SetEase(Ease.InSine));
			_sequence.AppendInterval(halfDuration);
			_sequence.Append(_canvasGroup.DOFade(0, quarterDuration).SetEase(Ease.OutSine));
		}

		#endregion

		[Serializable]
		private struct MessagePart
		{
			public UiMessage Type;
			public GameObject Part;
		}
	}
}