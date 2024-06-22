namespace Game.Iap
{
	using System;
	using System.Linq;
	using UniRx;
	using UnityEngine.Purchasing;
	using Zenject;
	using Unity.Services.Core;
	using Unity.Services.Core.Environments;
	using UnityEngine;
	using UnityEngine.Purchasing.Extension;
	using Module = Game.Logger.Module;

	public interface IPurchaseProcessor
	{
		PurchaseProcessingResult ProcessPurchase( Product product );
	}


	public interface IIapCoreListener
	{
		IStoreController		StoreController				{get;}
		IExtensionProvider		Extensions					{get;}

		bool HasProductInCatalog( string productId );
		Product GetProductFromCatalog( string productId );
	}


	public class IapCoreListener : IIapCoreListener, IInitializable, IDetailedStoreListener
	{
#region External

		[Inject] IPurchaseProcessor		_purchaseProcessor;
		[Inject] IIapCoreFacade			_iapCoreFacade;
		[Inject] IIapConfig             _iapConfig;

#endregion

		ProductCatalog		_catalog;


		async public void Initialize()
		{
			Game.Logger.Log( Module.Iap, "Start Initialize" );
			
			StandardPurchasingModule module			= StandardPurchasingModule.Instance();
#if UNITY_EDITOR
			//module.useFakeStoreUIMode				= FakeStoreUIMode.StandardUser;
#endif
			ConfigurationBuilder builder			= ConfigurationBuilder.Instance( module );
			_catalog								= ProductCatalog.LoadDefaultCatalog();

			// IAPConfigurationHelper is from Codeless, but that's OK
            IAPConfigurationHelper.PopulateConfigurationBuilder( ref builder, _catalog );

			try
			{
				var options = new InitializationOptions()
					.SetEnvironmentName( _iapConfig.Environment );

				Game.Logger.Log( Module.Iap, "UnityServices: Initialize" );

				await UnityServices.InitializeAsync( options ).ContinueWith( task => { 
					Game.Logger.Log( Module.Iap, $"UnityServices: Initialize Complete!" );
				} );;
			}
			catch (Exception exception)
			{
				Game.Logger.Log( Module.Iap, $"UnityServices Initialize Error: {exception.Message}" );

				Debug.LogException( exception );
			}

			Game.Logger.Log( Module.Iap, $"UnityPurchasing.Initialize: products count: {builder.products.Count}" );
			UnityPurchasing.Initialize( this, builder );
		}


#region IIapCoreListener


		public IStoreController			StoreController				{ get; private set; }
		public IExtensionProvider		Extensions					{ get; private set; }


		public bool HasProductInCatalog( string productId )				=> _catalog.allProducts.Any( p => p.id == productId );
		public Product GetProductFromCatalog( string productId )		=> StoreController.products.WithID( productId );

#endregion
#region IStoreListener

		public void OnInitialized( IStoreController controller, IExtensionProvider extensions )
		{
			StoreController			= controller;
			Extensions				= extensions;

			Game.Logger.Log( Module.Iap, "Initialized." );

			_iapCoreFacade.IsInitialized.Value		= true;
		}

		public void OnInitializeFailed( InitializationFailureReason error )
		{
			// Obsolete!
			Game.Logger.Log( Module.Iap, $"UnityServices Initialize Error: {error}" );

			throw new Exception( Game.Logger.Format( Module.Iap, $"Initialization failed: {error}." ) );
		}

		public void OnInitializeFailed(InitializationFailureReason error, string message)
		{
			Game.Logger.Log( Module.Iap, $"UnityServices Initialize Error: {error}. Message: {message}" );

			throw new Exception( Game.Logger.Format( Module.Iap, $"Initialization failed: {error}. Message: {message}" ) );
		}

		public PurchaseProcessingResult ProcessPurchase( PurchaseEventArgs purchaseEvent )
		{
			return _purchaseProcessor.ProcessPurchase( purchaseEvent.purchasedProduct );
		}


		public void OnPurchaseFailed( Product product, PurchaseFailureReason failureReason )
		{
			// Obsolete!

			// (!!!) Don't throw exceptions here! Otherwise (at least) fake-store-UI will not close after "Cancel" press. Not sure about real store UI...

			Game.Logger.LogError( Module.Iap, $"Purchase failed. Product: {product.definition.id}, reason: {failureReason}." );

			var productType = _iapConfig.BundleToId(product.definition.id);
			_iapCoreFacade.OnPurchaseFailed.Execute(productType);
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
		{
			// (!!!) Don't throw exceptions here! Otherwise (at least) fake-store-UI will not close after "Cancel" press. Not sure about real store UI...

			Game.Logger.LogError(
				Module.Iap, 
				$"Purchase failed. " +
				$"Product: {product.definition.id}, message: {failureDescription.message}, reason: {failureDescription.reason}."
			);

			var productType = _iapConfig.BundleToId(product.definition.id);
			_iapCoreFacade.OnPurchaseFailed.Execute(productType);
			_iapCoreFacade.OnPurchaseFailedDetailed.Execute(failureDescription);
		}

#endregion
	}
}

