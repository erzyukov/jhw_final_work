namespace Game.Iap
{
	using System;
	using UnityEngine.Localization;
	using UnityEngine.Localization.SmartFormat.PersistentVariables;
	using UnityEngine.Localization.Tables;
	using YG;

	public class IapYandexCore : IapCore
	{
		private const string PriceVariableKey = "price";

		public override void Buy( EIapProduct product )
		{
			if (IapShopProfile.BoughtProducts.Contains( product ))
				return;

			if (IapConfig.TryGetBundle( product, out string bundleId ) == false)
				return;

			YandexGame.BuyPayments( bundleId );
		}

		//YandexCurrency
		public override string GetLocalizedPrice( EIapProduct product )
		{
			if (IapConfig.TryGetIapData( product, out var data ) == false)
				return string.Empty;

			var s = new LocalizedString( "StringTableCollection" , "YandexCurrency");
			s.Add( PriceVariableKey, new StringVariable { Value = data.Price.ToString() } );


			return s.GetLocalizedString();
		}
	}
}
