namespace Game.Iap
{
	using UnityEngine;
	using UnityEngine.Purchasing;
	using Zenject;
	using Logger = Game.Logger;
	using Module = Game.Logger.Module;


	public interface IIapCore
	{
		bool IsRestoreMode		{get;}

		void Buy( EIapProduct product );
		void Restore();
		bool IsBought(EIapProduct productType);
		string GetLocalizedPrice( EIapProduct product );
	}


	public class IapCore : MonoBehaviour, IIapCore, IPurchaseProcessor
	{
#region External

		[Inject] protected IIapCoreFacade		_iapCoreFacade;
		[Inject] protected IIapConfig			_iapConfig;
		[Inject] protected IIapCoreListener		_iapCoreListener;

		protected IStoreController		StoreController			=> _iapCoreListener.StoreController;
		protected IExtensionProvider	Extensions				=> _iapCoreListener.Extensions;

#endregion

		protected Product	_product;


#region IIapCore

		public bool IsRestoreMode		{ get; private set; }			= true;


		public void Buy( EIapProduct product )
		{
			Logger.Log( Module.Iap, $"Request for purchase initiation: {product}." );

			IsRestoreMode		= false;

			if (StoreController == null)
			{
				_iapCoreFacade.OnPurchaseFailed.Execute(product);
				Logger.LogError( Module.Iap, $"StoreController not initialized! On initiate purchase {product}.");
				return;
			}

			if (_iapConfig.TryGetBundle( product, out string productId ))
			{
				Logger.Log( Module.Iap, $"Product ID found for: {product}, ID: {productId}." );

				StoreController.InitiatePurchase( productId );
			}
			else
			{
				Logger.LogError( Module.Iap, $"Product ID not found for: {product}." );
			}
		}


        public void Restore()
        {
			Logger.Log( Module.Iap, "Request for purchases restore." );

			IsRestoreMode		= true;

			if (
				Application.platform == RuntimePlatform.WSAPlayerX86	||
			    Application.platform == RuntimePlatform.WSAPlayerX64	||
			    Application.platform == RuntimePlatform.WSAPlayerARM)
			{
				Extensions
					.GetExtension< IMicrosoftExtensions >()
					.RestoreTransactions();
			}
			else if (
				Application.platform == RuntimePlatform.IPhonePlayer	||
				Application.platform == RuntimePlatform.OSXPlayer		||
				Application.platform == RuntimePlatform.tvOS)
			{
				Extensions
					.GetExtension< IAppleExtensions >()
					.RestoreTransactions( OnTransactionsRestored );
			}
			else if (
				Application.platform == RuntimePlatform.Android &&
				StandardPurchasingModule.Instance().appStore == AppStore.GooglePlay)
			{
				Extensions
					.GetExtension< IGooglePlayStoreExtensions >()
					.RestoreTransactions( OnTransactionsRestored );
			}
			else
			{
				Logger.LogError( Module.Iap, Application.platform + " is not a supported platform for IAP restore." );
			}
        }


		void OnTransactionsRestored( bool success, string messege )
		{
			Logger.Log( Module.Iap, $"OnTransactionsRestored: {success}. Message: {messege}." );

			_iapCoreFacade.OnRestoreProcessFinish.Execute( success );
		}

		public bool IsBought(EIapProduct productType)
		{
			if (_iapConfig.TryGetBundle(productType, out string productId))
			{
				var product = _iapCoreListener.GetProductFromCatalog(productId);

				Logger.Log( Module.Iap, $"IsBought check: Product hasReceipt: {product.hasReceipt}. ({product.definition.id})" );

				return product.hasReceipt;
			}

			return false;
		}

#endregion
#region IPurchaseProcessor

		public PurchaseProcessingResult ProcessPurchase( Product product )
		{
			SetProduct( product );

			var result			= ProductProcessPurchase( product );

			// This is a trick
			// https://forum.unity.com/threads/how-to-differentiate-processpurchase-callbacks-on-client-side.1298010/
			IsRestoreMode		= true;

			return result;
		}

#endregion


		protected virtual PurchaseProcessingResult ProductProcessPurchase( Product product )
		{
			return ProcessPurchase_GetProduct();
		}


		protected PurchaseProcessingResult ProcessPurchase_GetProduct()
		{
			GetProduct();

			NullifyProduct();
			return PurchaseProcessingResult.Complete;
		}


		protected void GetProduct()
		{
			_iapCoreFacade.OnBoughtOrRestored.Execute( _product );

			Logger.Log( Module.Iap, $"Product bought: {_product.definition.id}." );
		}


		protected void NullifyProduct()						=> SetProduct( null );
		protected void SetProduct( Product product )		=> _product		= product;

		public virtual string GetLocalizedPrice( EIapProduct product )
		{
			//if (IapConfig.TryGetIapData( product, out var data ) == false)
				//return string.Empty;

			//return data.Price.ToString();

			return string.Empty;
		}
	}
}