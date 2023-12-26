namespace Game.Tutorial
{
	using UnityEngine;
	using TMPro;
	using System;
	using System.Linq;
	using DG.Tweening;
	using UnityEngine.UI;
	using UniRx;

	public interface IDialogHint
	{
		IObservable<Unit> NextMessageButtonClicked { get; }
		void SetMessage(string value);
		void SetActive(bool value);
		void SetPlace(DialogHintPlace place);
		void SetNextMessageButtonActive(bool value);
		void SetNextMessageIndicatorActive(bool value);
	}

    public class DialogHint : MonoBehaviour, IDialogHint
	{
		[SerializeField] Image _buttonImage;
		[SerializeField] Button _continueButton;
		[SerializeField] Transform _dialogTransform;
		[SerializeField] private TextMeshProUGUI _message;
		[SerializeField] Image _nextMessageIndicator;
		[SerializeField] float _indicatorDuration;
		[SerializeField] private HintPlace[] hintPlaces;

		Tween _indicatorTween;

		#region IDialogHint

		public IObservable<Unit> NextMessageButtonClicked => _continueButton.OnClickAsObservable();

		public void SetActive(bool value) => 
			gameObject.SetActive(value);

		public void SetMessage(string value) => 
			_message.text = value;

		public void SetPlace(DialogHintPlace place)
		{
			Transform parent = hintPlaces.Where(hintPlace => hintPlace.Place == place).Select(hintPlace => hintPlace.Parent).First();
			_dialogTransform.SetParent(parent, false);
		}

		public void SetNextMessageButtonActive(bool value)
		{
			_buttonImage.raycastTarget = value;
		}

		public void SetNextMessageIndicatorActive(bool value)
		{
			_nextMessageIndicator.gameObject.SetActive(value);

			if (value)
			{
				_indicatorTween = _nextMessageIndicator
					.DOFade(0, _indicatorDuration)
					.SetEase(Ease.OutSine)
					.SetLoops(-1, LoopType.Yoyo);
			}
			else
			{
				_indicatorTween.Kill();
				_indicatorTween = null;
			}
		}

		#endregion

		[Serializable]
		struct HintPlace
		{
			public DialogHintPlace Place;
			public Transform Parent;
		}
	}
}