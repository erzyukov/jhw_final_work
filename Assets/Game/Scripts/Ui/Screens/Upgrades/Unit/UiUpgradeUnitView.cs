namespace Game.Ui
{
	using Game.Utilities;
	using System;
	using TMPro;
	using UniRx;
	using UnityEngine;
	using UnityEngine.Localization.Components;
	using UnityEngine.Localization.SmartFormat.PersistentVariables;
	using UnityEngine.UI;
	using Zenject;

	public interface IUiUpgradeUnitView
	{
		IObservable<Unit> SelectButtonClicked { get; }
		IObservable<Unit> UpgradeButtonClicked { get; }
		IObservable<Unit> AdsButtonClicked { get; }
		IObservable<Unit> UnlockButtonClicked { get; }
		Button SelectButton { get; }
		AdButton UpgradeButton { get; }

		void SetIcon( Sprite value );
		void SetLevel( int value );
		void SetTitle( string value );
		void SetPrice( int value );
		void SetParent( Transform unitsContainer );
		void SetSelected( bool value );
		void SetSelectInteractable( bool value );
		void SetUpgradeInteractable( bool value );
		void SetBlocked( bool value );
		void SetLevelNumberActive(bool value);
		void SetUnlockLevelActive( bool value );
		void SetLockedButtonActive( bool value );
		void SetUnlockLevel( int value );
		void SetUnlockButtonActive( bool value );
		void SetUnlockPrice( int value );
	}

	public class UiUpgradeUnitView : MonoBehaviour, IUiUpgradeUnitView
	{
		[SerializeField] private Image					_icon;
		[SerializeField] private Button					_selectButton;
		[SerializeField] private RectTransform			_selectButtonRect;
		[SerializeField] private AdButton				_upgradeButton;
		[SerializeField] private LocalizeStringEvent	_localizedLevel;
		[SerializeField] private TextMeshProUGUI		_level;
		[SerializeField] private TextMeshProUGUI		_title;
		[SerializeField] private LocalizeStringEvent	_localizedPrice;
		[SerializeField] private float					_activeHeightDelta;
		[SerializeField] private float					_defaultYPos;
		[SerializeField] private GameObject				_selection;
		[SerializeField] private Image					_textDelimeter;
		[SerializeField] private Color					_activeColor;
		[SerializeField] private Color					_inactiveColor;
		[SerializeField] private Material				_blockedImageMaterial;
		[SerializeField] private GameObject				_defaultButton;
		[SerializeField] private GameObject				_lockedButton;
		[SerializeField] private GameObject				_unlockedLevel;
		[SerializeField] private LocalizeStringEvent	_localizedUnlockLevel;
		[SerializeField] private Button					_unlockButton;
		[SerializeField] private LocalizeStringEvent	_unlockPrice;
		[SerializeField] private float                  _lockedIconAlpha;

		private const string levelVariable		= "level";
		private const string priceVariable		= "price";

		public IObservable<Unit> SelectButtonClicked		=> _selectButton.OnClickAsObservable();
		public IObservable<Unit> UpgradeButtonClicked		=> _upgradeButton.Clicked;
		public IObservable<Unit> AdsButtonClicked			=> _upgradeButton.AdClicked;
		public IObservable<Unit> UnlockButtonClicked		=> _unlockButton.OnClickAsObservable();
		public Button SelectButton							=> _selectButton;
		public AdButton UpgradeButton						=> _upgradeButton;

		// Fix unrecognized scaling for screen with free aspect rate
		public void OnEnable()			=> transform.localScale = Vector3.one;

		public void SetIcon( Sprite value )			=> _icon.sprite = value;

		public void SetLevel( int value ) =>
			( _localizedLevel.StringReference[levelVariable] as IntVariable ).Value = value;

		public void SetTitle( string value ) =>
			_title.text = value;

		public void SetPrice( int value ) =>
			( _localizedPrice.StringReference[priceVariable] as IntVariable ).Value = value;

		public void SetSelected( bool value )
		{
			_textDelimeter.color = value ? _activeColor : _inactiveColor;
			_level.color = value ? _activeColor : _inactiveColor;
			_title.color = value ? _activeColor : _inactiveColor;
			_selection.SetActive( value );

			Vector2 offsetMax = _selectButtonRect.offsetMax;
			offsetMax.y = _defaultYPos + (value ? _activeHeightDelta : 0);
			_selectButtonRect.offsetMax = offsetMax;
		}

		public void SetSelectInteractable( bool value ) =>
			_selectButton.interactable = value;

		public void SetUpgradeInteractable( bool value ) =>
			_upgradeButton.SetDefaultInteractable( value );

		public void SetParent( Transform unitsContainer ) => transform.SetParent( unitsContainer );

		public void SetUnlockLevelActive( bool value ) =>
			_unlockedLevel.SetActive( !value );

		public void SetUnlockLevel( int value ) =>
			( _localizedUnlockLevel.StringReference[levelVariable] as IntVariable ).Value = value;

		public void SetLevelNumberActive(bool value) =>
			_level.gameObject.SetActive( value );

		public void SetBlocked( bool value )
		{
			_icon.material = ( value ) ? _blockedImageMaterial : null;
			_localizedPrice.gameObject.SetActive( !value );
			_defaultButton.SetActive( !value );
			_lockedButton.SetActive( value );
			_icon.color = _icon.color.WithAlpha( value ? _lockedIconAlpha : 1 );
		}

		public void SetLockedButtonActive( bool value ) =>
			_lockedButton.SetActive( value );

		public void SetUnlockButtonActive( bool value ) => 
			_unlockButton.gameObject.SetActive( value );

		public void SetUnlockPrice( int value ) =>
			( _unlockPrice.StringReference[priceVariable] as IntVariable ).Value = value;

		public class Factory : PlaceholderFactory<UiUpgradeUnitViewFactory.Args, IUiUpgradeUnitView> { }
	}
}
