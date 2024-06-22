namespace Game.Iap
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using Game.Configs;


	public interface IIapConfig: IIapCoreConfig
	{
		void SetPricesAB(bool isNewPrices);
	}


	[CreateAssetMenu(fileName = "IAP", menuName = "Configs/Iap", order = ( int ) Config.Iap)]
	public class IapConfig : IapCoreConfig, IIapConfig
	{

		[SerializeField] Dictionary<EIapProduct, IapData>   ProductsNew     = new Dictionary<EIapProduct, IapData>();

		bool    _isNewPrices;

#region IIapConfig

		public void SetPricesAB( bool isNewPrices )			=> _isNewPrices = isNewPrices;

		public override EIapProduct BundleToId( string bundle )
		=>
			Products.Concat( ProductsNew ).First( pair => pair.Value.Bundle == bundle ).Key;

#endregion


		protected override bool TryGetIapData( EIapProduct id, out IapData iapData )
		{
			var takeFrom        =
									_isNewPrices && ProductsNew.ContainsKey( id ) ? ProductsNew :	// B-version
									Products.ContainsKey( id ) ? Products :							// A-version or single version only (no A/B)
									null;                                                           // Error. Not found.

			bool isFound        = takeFrom != null;
			iapData				= isFound ? takeFrom[id] : null;

			return isFound;
		}
	}
}

