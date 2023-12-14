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

		public void SetIcon(Sprite value) => 
			_icon.sprite = value;

		public void SetLevel(string value) =>
			_level.text = value;

		public void SetParent(Transform parent) =>
			transform.SetParent(parent);

		public IObservable<Unit> SelectButtonClicked => 
			_selectButton.OnClickAsObservable();

		public IObservable<Unit> UpgradeButtonClicked => 
			_upgradeButton.OnClickAsObservable();

	}
}