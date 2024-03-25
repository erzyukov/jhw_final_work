namespace Game.Ui
{
	using System.Collections.Generic;
	using UniRx;
	using UnityEngine;

	public interface IUiIapShopScreen : IUiScreen
	{

	}

    public class UiIapShopScreen : UiScreen, IUiIapShopScreen
    {
		public override Screen Screen => Screen.IapShop;



	}
}
