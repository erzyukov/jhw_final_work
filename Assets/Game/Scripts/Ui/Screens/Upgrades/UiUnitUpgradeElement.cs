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
		[SerializeField] private TextMeshProUGUI _level;
		[SerializeField] private Button _selectButton;
		[SerializeField] private Button _upgradeButton;
		[SerializeField] private TextMeshProUGUI _upgradePrice;

		public Button SelectButton => _selectButton;

		public Button UpgradeButton => _upgradeButton;

		public void SetIcon(Sprite value) =>
			_icon.sprite = value;

		public void SetLevel(string value) =>
			_level.text = value;

		public void SetPrice(string value) =>
			_upgradePrice.text = $"<sprite=0> {value}";

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