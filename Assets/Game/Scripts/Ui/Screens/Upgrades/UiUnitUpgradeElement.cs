namespace Game.Ui
{
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;
	using System;
	using UniRx;

	public class UiUnitUpgradeElement : MonoBehaviour
	{
		[SerializeField] private Image _icon;
		[SerializeField] private Button _selectButton;
		[SerializeField] private RectTransform _selectButtonRect;
		[SerializeField] private Button _upgradeButton;
		[SerializeField] private TextMeshProUGUI _level;
		[SerializeField] private TextMeshProUGUI _title;
		[SerializeField] private TextMeshProUGUI _upgradePrice;
		[SerializeField] private string _upgradePricePrefix;
		[SerializeField] private float _activeHeightDelta;
		[SerializeField] private float _defaultYPos;
		[SerializeField] private GameObject _selection;
		[SerializeField] private Image _textDelimeter;
		[SerializeField] private Color _activeColor;
		[SerializeField] private Color _inactiveColor;

		public Button SelectButton => _selectButton;

		public Button UpgradeButton => _upgradeButton;

		public void SetIcon(Sprite value) =>
			_icon.sprite = value;

		public void SetLevel(string value) =>
			_level.text = value;

		public void SetTitle(string value) =>
			_title.text = value;

		public void SetPrice(string value) =>
			_upgradePrice.text = $"{_upgradePricePrefix}{value}";

		public void SetSelected(bool value)
		{
			_textDelimeter.color = value ? _activeColor : _inactiveColor;
			_level.color = value ? _activeColor : _inactiveColor;
			_title.color = value ? _activeColor : _inactiveColor;
			_selection.SetActive(value);

			Vector2 offsetMax = _selectButtonRect.offsetMax;
			offsetMax.y = _defaultYPos + (value ? _activeHeightDelta : 0);
			_selectButtonRect.offsetMax = offsetMax;
		}

		public void SetSelectInteractable(bool value) =>
			_selectButton.interactable = value;

		public void SetUpgradeInteractable(bool value) =>
			_upgradeButton.interactable = value;

		public IObservable<Unit> SelectButtonClicked =>
			_selectButton.OnClickAsObservable();

		public IObservable<Unit> UpgradeButtonClicked =>
			_upgradeButton.OnClickAsObservable();
	}
}