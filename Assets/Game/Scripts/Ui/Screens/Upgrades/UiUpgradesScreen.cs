namespace Game.Ui
{
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;
	using Game.Units;

	public interface IUiUpgradesScreen : IUiScreen
	{
		Transform UnitsContainer { get; }
		UiUnitUpgradeElement UnitElementPrefab { get; }
		GameObject UnitUnavailableDummyPrefab { get; }
		void SetIcon(Sprite value);
		void SetName(string value);
		void SetLevel(string value);
		void SetHealthValue(string value, string delta);
		void SetDamageValue(string value, string delta);
		void SetSpeedValue(string value);
		void SetRangeValue(string value);
	}

	public class UiUpgradesScreen : UiScreen, IUiUpgradesScreen
	{
		[SerializeField] private Image _unitIcon;
		[SerializeField] private TextMeshProUGUI _unitName;
		[SerializeField] private TextMeshProUGUI _unitLevel;
		[SerializeField] private TextMeshProUGUI _unitHealthValue;
		[SerializeField] private TextMeshProUGUI _unitDamageValue;
		[SerializeField] private TextMeshProUGUI _unitSpeedValue;
		[SerializeField] private TextMeshProUGUI _unitRangeValue;
		[SerializeField] private Transform _unitsContainer;
		[SerializeField] private UiUnitUpgradeElement _unitUpgradeElementPrefab;
		[SerializeField] private GameObject _unitUnavailableDummyPrefab;
		[SerializeField] private Color _parameterDeltaColor;

		public override Screen Screen => Screen.Upgrades;

		#region IUiLobbyScreen

		public Transform UnitsContainer => _unitsContainer;

		public UiUnitUpgradeElement UnitElementPrefab => _unitUpgradeElementPrefab;

		public GameObject UnitUnavailableDummyPrefab => _unitUnavailableDummyPrefab;

		public void SetIcon(Sprite value) =>
			_unitIcon.sprite = value;

		public void SetName(string value) =>
			_unitName.text = value;

		public void SetLevel(string value) =>
			_unitLevel.text = value;

		public void SetHealthValue(string value, string delta) =>
		_unitHealthValue.text = $"{value} <color=#{ColorUtility.ToHtmlStringRGB(_parameterDeltaColor)}>+ {delta}</color>";

		public void SetDamageValue(string value, string delta) =>
		_unitDamageValue.text = $"{value} <color=#{ColorUtility.ToHtmlStringRGB(_parameterDeltaColor)}>+ {delta}</color>";

		public void SetSpeedValue(string value) =>
			_unitSpeedValue.text = value;

		public void SetRangeValue(string value) =>
			_unitRangeValue.text = value;

		#endregion
	}
}