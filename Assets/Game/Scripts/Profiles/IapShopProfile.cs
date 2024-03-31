namespace Game.Profiles
{
	using System;
	using System.Collections.Generic;
	using UniRx;

	[Serializable]
	public class IapShopProfile
	{
		public BoolReactiveProperty NoAdsProduct = new();

		public List<EIapProduct> BoughtProducts = new();
	}
}
