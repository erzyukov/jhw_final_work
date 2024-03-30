namespace Game.Iap
{
	using Sirenix.OdinInspector;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.Localization.Tables;

	[Serializable]
	public class IapData
	{
		public string Bundle;
		public float Price;
		public bool IsConsumable;
	}

	public interface IIapCoreConfig
	{
		bool TryGetBundle( EIapProduct id, out string bundle );
		bool TryGetIapData( EIapProduct id, out IapData iapData );
		EIapProduct BundleToId( string bundle );
	}

	public class IapCoreConfig : SerializedScriptableObject, IIapCoreConfig
	{
		[SerializeField] protected Dictionary<EIapProduct, IapData>		Products				= new Dictionary<EIapProduct, IapData>();

		#region IIapCoreConfig

		public bool TryGetBundle( EIapProduct id, out string bundle )
		{
			bool found      = TryGetIapData( id, out IapData iapData );
			bundle			= iapData.Bundle;

			return found;
		}

		public virtual bool TryGetIapData( EIapProduct id, out IapData iapData ) =>
			Products.TryGetValue( id, out iapData );

		public virtual EIapProduct BundleToId( string bundle ) =>
			Products.First( pair => pair.Value.Bundle == bundle ).Key;

		#endregion
	}
}
