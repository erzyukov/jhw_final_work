namespace Game.Ui
{
	using System.Collections.Generic;
	using UniRx;
	using UnityEngine;

	public interface IUiIapShopScreen : IUiScreen
	{
		List<UiIapShopElement> Products { get; }
	}

    public class UiIapShopScreen : UiScreen, IUiIapShopScreen
    {
		[SerializeField] private List<UiIapShopElement> _products;

		public override Screen Screen => Screen.IapShop;

		#region IUiIapShopScreen

		public List<UiIapShopElement> Products => _products;

		#endregion

	}
}
