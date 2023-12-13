namespace Game.Ui
{
	using Game.Core;
	using System;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	[RequireComponent(typeof(Button), typeof(RectTransform))]
	public class UiMainMenuButton : MonoBehaviour
    {
		[SerializeField] private Sprite _defaultIcon;
		[SerializeField] private Image _icon;
		[SerializeField] private GameState _targetGameState;
		[SerializeField] private Image _buttonBackground;
		[SerializeField] private GameObject _title;

		private Button _button;
		private RectTransform _rectTransform;

		private void Awake()
		{
			_button = GetComponent<Button>();
			_rectTransform = GetComponent<RectTransform>();
		}

		public GameState TargetGameState => _targetGameState;

		public IObservable<Unit> ButtonClicked => _button.OnClickAsObservable();

		public void SetBackgroundColor(Color value) => _buttonBackground.color = value;

		public void SetIcon(Sprite lockedIcon) => _icon.sprite = lockedIcon;
		
		public void SetDefaultIcon() => _icon.sprite = _defaultIcon;

		public void SetTitleActive(bool value) => _title.SetActive(value);

		public void SetInteractable(bool value) => _button.interactable = value;

		public void SetHeight(float value) =>
			_rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, value);
	}
}