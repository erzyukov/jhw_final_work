namespace Game.Ui
{
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;
	using UnityEngine.Localization.Components;
	using UnityEngine.Localization.SmartFormat.PersistentVariables;

	public interface IUiUpgradesScreen : IUiScreen
	{
		Transform UnitsContainer { get; }
		void SetIcon(Sprite value);
		void SetIconMaterial(Material value);
		void SetName(string value);
		void SetLevel(int value);
		void SetLevelActive( bool value );
		void SetHealthValue(string value, string delta);
		void SetDamageValue(string value, string delta);
		void SetSpeedValue(string value);
		void SetRangeValue(string value);
	}

	public class UiUpgradesScreen : UiScreen, IUiUpgradesScreen
	{
		[SerializeField] private Image _unitIcon;
		[SerializeField] private TextMeshProUGUI _unitName;
		[SerializeField] private LocalizeStringEvent _localizedLevel;
		[SerializeField] private TextMeshProUGUI _unitHealthValue;
		[SerializeField] private TextMeshProUGUI _unitDamageValue;
		[SerializeField] private TextMeshProUGUI _unitSpeedValue;
		[SerializeField] private TextMeshProUGUI _unitRangeValue;
		[SerializeField] private Transform _unitsContainer;
		[SerializeField] private UiUnitUpgradeElement _unitUpgradeElementPrefab;
		[SerializeField] private GameObject _unitUnavailableDummyPrefab;
		[SerializeField] private Color _parameterDeltaColor;

		private const string levelVariable		= "level";

		public override Screen Screen => Screen.Upgrades;

		#region IUiLobbyScreen

		public Transform UnitsContainer => _unitsContainer;

		public void SetIcon( Sprite value ) =>
			_unitIcon.sprite = value;

		public void SetIconMaterial( Material value ) =>
			_unitIcon.material = value;

		public void SetName( string value ) =>
			_unitName.text = value;

		public void SetLevel( int value ) =>
			( _localizedLevel.StringReference[levelVariable] as IntVariable ).Value = value;

		public void SetLevelActive( bool value ) =>
			_localizedLevel.gameObject.SetActive( value );

		public void SetHealthValue( string value, string delta ) =>
			_unitHealthValue.text = $"{value} <color=#{ColorUtility.ToHtmlStringRGB( _parameterDeltaColor )}>+ {delta}</color>";

		public void SetDamageValue( string value, string delta ) =>
			_unitDamageValue.text = $"{value} <color=#{ColorUtility.ToHtmlStringRGB( _parameterDeltaColor )}>+ {delta}</color>";

		public void SetSpeedValue( string value ) =>
			_unitSpeedValue.text = value;

		public void SetRangeValue( string value ) =>
			_unitRangeValue.text = value;

		#endregion
	}
}