namespace Game.Ui
{
	using UnityEngine;
	using TMPro;
	using UnityEngine.UI;
	using System;
	using UniRx;

	public class UiIapShopElement : MonoBehaviour
	{
		[SerializeField] private EIapProduct _iapProduct;
		[SerializeField] private TextMeshProUGUI _price;
		[SerializeField] private Button _button;
		[SerializeField] private GameObject _activeState;
		[SerializeField] private GameObject _soldState;

		public EIapProduct IapProduct => _iapProduct;

		public void SetPrice( string value ) =>
			_price.text = value;

		public void SetBought()
		{
			_activeState.SetActive( false );
			_soldState.SetActive( true );
			_button.interactable = false;
		}

		public IObservable<Unit> Clicked => _button.OnClickAsObservable();
	}
}
