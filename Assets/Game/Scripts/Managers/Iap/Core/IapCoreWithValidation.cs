namespace Game.Iap
{
	using System;
	using UnityEngine.Purchasing;
	using Logger = Game.Logger;
	using Module = Game.Logger.Module;
	using UnityEngine.Purchasing.Security;

	public class IapCoreWithValidation : IapCore
	{

		protected override PurchaseProcessingResult ProductProcessPurchase( Product product )
		{
			bool isValidPurchase	= true;

			if (IsCurrentStoreSupportedByValidator())
			{
#if !UNITY_EDITOR
				var validator       = 
					new CrossPlatformValidator( GooglePlayTangle.Data(), null, Application.identifier );

				try
				{
					// On Google Play, result has a single product ID.
					// On Apple stores, receipts contain multiple products.
					var result		= validator.Validate( product.receipt );

					OnValidateSucces( result );
				}
				catch (IAPSecurityException)
				{
					OnValidateError( "Invalid receipt, not unlocking content" );
					isValidPurchase			= false;
				}
#endif
			}

			if (isValidPurchase)
				return ProcessPurchase_GetProduct();
			else
				return PurchaseProcessingResult.Pending;
		}

		public void OnValidateSucces( IPurchaseReceipt[] result )
		{
			GetProduct();

			_iapCoreFacade.OnValidatedPurchase.Execute( new PurchaseData { 
				Currency		= _product.metadata.isoCurrencyCode,
				CentsAmount		= (int)Math.Ceiling( _product.metadata.localizedPrice * 100 ),
				ItemType		= _product.definition.type.ToString(),
				ItemId			= _product.definition.id,
				CartType		= "Shop",
				Receipt			= _product.receipt
			});

			foreach (IPurchaseReceipt productReceipt in result)
				Logger.Log( 
					Module.Iap, 
					$"OnValidateSucces: ID: {productReceipt.productID}, Date: {productReceipt.purchaseDate}, transactionID: {productReceipt.transactionID}"
				);

			FinishValidate();
		}

		public void OnValidateError( string message )
		{
			Logger.Log( Module.Iap, $"OnValidateError: {message}" );

			var productType = _iapConfig.BundleToId(_product.definition.id);
			_iapCoreFacade.OnPurchaseFailed.Execute(productType);

			FinishValidate();
		}

        private bool IsCurrentStoreSupportedByValidator()
        {
            //The CrossPlatform validator only supports the GooglePlayStore and Apple's App Stores.
            return IsGooglePlayStoreSelected() || IsAppleAppStoreSelected();
        }

        private bool IsGooglePlayStoreSelected()
        {
            var currentAppStore = StandardPurchasingModule.Instance().appStore;
            return currentAppStore == AppStore.GooglePlay;
        }

        private bool IsAppleAppStoreSelected()
        {
            var currentAppStore = StandardPurchasingModule.Instance().appStore;
            return currentAppStore == AppStore.AppleAppStore ||
                currentAppStore == AppStore.MacAppStore;
        }

		private void FinishValidate()
		{
			StoreController.ConfirmPendingPurchase( _product );		// (!) Call it BEFORE NullifyProduct()
			NullifyProduct();
		}
	}
}

