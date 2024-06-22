namespace Game.Iap
{
	using Sirenix.OdinInspector;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;


	[Serializable]
	public class IapData
	{
		public string   Bundle;
	}


	public interface IIapCoreConfig
	{
		string GooglePublicKey		{get;}
		string Environment			{get;}

		bool TryGetBundle( EIapProduct id, out string bundle );
		EIapProduct BundleToId( string bundle );
	}


	public class IapCoreConfig : SerializedScriptableObject, IIapCoreConfig
	{
		[SerializeField] string											_googlePublicKey;
		[SerializeField] string											_environment;
		[SerializeField] protected Dictionary<EIapProduct, IapData>		Products				= new Dictionary<EIapProduct, IapData>();


#region IIapConfig

		public string GooglePublicKey		=> _googlePublicKey;

		public string Environment			=> _environment;

		public bool TryGetBundle( EIapProduct id, out string bundle )
		{
			bool found      = TryGetIapData( id, out IapData iapData );
			bundle			= iapData.Bundle;

			return found;
		}


		public virtual EIapProduct BundleToId( string bundle )
		=>
			Products.First( pair => pair.Value.Bundle == bundle ).Key;

#endregion


		protected virtual bool TryGetIapData( EIapProduct id, out IapData iapData )
		=>
			Products.TryGetValue( id, out iapData );
	}
}

