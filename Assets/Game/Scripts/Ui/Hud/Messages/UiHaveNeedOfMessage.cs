namespace Game.Ui
{
	using System;
	using UnityEngine;
	using DG.Tweening;

	public enum NeedMessage
	{
		SummonCurrency,
		ChestKeys
	}

	public interface IUiHaveNeedOfMessage
	{
		void ShowMessage(NeedMessage type);
	}

	public class UiHaveNeedOfMessage : MonoBehaviour, IUiHaveNeedOfMessage
	{
		[SerializeField] private float _showDuration;
		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private MessagePart[] _messageParts;

		Sequence _sequence;

		#region IHaveNeedOfMessage

		public void ShowMessage(NeedMessage type)
		{
			_sequence?.Kill();

			foreach (var part in _messageParts)
                part.Part.SetActive(part.Type == type);

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
			public NeedMessage Type;
			public GameObject Part;
		}
	}
}