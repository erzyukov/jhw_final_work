namespace Game
{
	using Game.Utilities;
	using System;
	using TMPro;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public class AdButton : MonoBehaviour
    {
		[SerializeField] private Button _adsButton;
		[SerializeField] private Button _defaultButton;
		[SerializeField] private TextMeshProUGUI _defaultButtonText;

		public Button DefaultButton => _defaultButton;

		public IObservable<Unit> Clicked =>
			_defaultButton.OnClickAsObservable();

		public IObservable<Unit> AdClicked =>
			_adsButton.OnClickAsObservable();

		public void SetAdActive( bool value )
		{
			_adsButton.gameObject.SetActive( value );
			_defaultButton.interactable = !value;
			_defaultButtonText.color = _defaultButtonText.color.WithAlpha( value ? 0.2f : 1 );
		}

		public void SetDefaultValue( string value ) =>
			_defaultButtonText.text = value;

		public void SetDefaultInteractable(bool value) =>
			_defaultButton.interactable = value;
    }
}
