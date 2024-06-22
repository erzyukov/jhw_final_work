namespace Game.Iap
{
	using System;
	using UniRx;
	using UnityEngine;
	using UnityEngine.Purchasing;
	using Zenject;
	using UnityEngine.Purchasing.Extension;

	public struct PurchaseData
	{
		public string   Currency;
		public int      CentsAmount;
		public string   ItemType;
		public string   ItemId;
		public string   CartType;
		public string   Receipt;
	}


	public interface IIapFacade
	{
		BoolReactiveProperty IsInitialized { get; }
		ReactiveCommand<bool> OnRestoreProcessFinish { get; }
		ReactiveCommand<Product> OnBoughtOrRestored { get; }
		ReactiveCommand<PurchaseData> OnValidatedPurchase { get; }
		ReactiveCommand<EIapProduct> OnPurchaseFailed { get; }
		ReactiveCommand<PurchaseFailureDescription> OnPurchaseFailedDetailed { get; }

		bool IsRestoreMode { get; }
		bool Sandbox { get; set; }
		bool NoValidation { get; set; }
		void Buy(EIapProduct product);
		string GetLocalizedPriceString(EIapProduct iapProduct);
		decimal GetLocalizedPrice(EIapProduct iapProduct);
		string GetLocalizedIcoCurrencyCode(EIapProduct iapProduct);
		int GetPayoutQuantity(EIapProduct iapProduct);
		void Restore();
	}


	public class IapFacade: IIapFacade, IInitializable, IDisposable
	{
#region External

		[Inject] IIapCoreFacade			_iapCoreFacade;
		[Inject] IIapConfig				_iapConfig;

#endregion
		
		readonly CompositeDisposable _lifetimeDisposables = new CompositeDisposable();


		public void Initialize()
		{
			_iapCoreFacade.IsInitialized
				.Where( v => v )
				.First()
				.Subscribe( _ => IsInitialized.Value = true )
				.AddTo( _lifetimeDisposables );
		}


		public void Dispose()		=> _lifetimeDisposables.Clear();


#region IIapFacade

		public BoolReactiveProperty IsInitialized					{ get; } = new BoolReactiveProperty();

		public ReactiveCommand<EIapProduct> OnPurchaseFailed		=> _iapCoreFacade.OnPurchaseFailed;
		
		public ReactiveCommand<bool> OnRestoreProcessFinish			=> _iapCoreFacade.OnRestoreProcessFinish;

		public ReactiveCommand<Product> OnBoughtOrRestored			=> _iapCoreFacade.OnBoughtOrRestored;

		public ReactiveCommand<PurchaseData> OnValidatedPurchase	=> _iapCoreFacade.OnValidatedPurchase;

		public bool IsRestoreMode									=> _iapCoreFacade.IsRestoreMode;
		
		public ReactiveCommand<PurchaseFailureDescription> OnPurchaseFailedDetailed => _iapCoreFacade.OnPurchaseFailedDetailed;

		public bool Sandbox
		{
			get => _iapCoreFacade.Sandbox;
			set => _iapCoreFacade.Sandbox = value;
		}

		public bool NoValidation
		{
			get => _iapCoreFacade.NoValidation;
			set => _iapCoreFacade.NoValidation = value;
		}

		public void Buy(EIapProduct product)						=> _iapCoreFacade.Buy(product);

		public string GetLocalizedPriceString(EIapProduct iapProduct)
		=>
			TryGetProduct(iapProduct, out Product product) ?
			product.metadata.localizedPriceString :
			"ERROR";

		public int GetPayoutQuantity(EIapProduct iapProduct)
		=>
			TryGetProduct(iapProduct, out Product product) ?
			RoundPayoutQuantityToInt(product) :
			-1;

		public decimal GetLocalizedPrice(EIapProduct iapProduct)
		=>
			TryGetProduct(iapProduct, out Product product) ?
			product.metadata.localizedPrice :
			decimal.MinusOne;

		public string GetLocalizedIcoCurrencyCode(EIapProduct iapProduct)
		=>
			TryGetProduct(iapProduct, out Product product) ?
			product.metadata.isoCurrencyCode :
			"";

		public void Restore()							=> _iapCoreFacade.Restore();

#endregion


		bool TryGetProduct(EIapProduct iapProduct, out Product product)
		{
			product = null;

			if (!_iapConfig.TryGetBundle(iapProduct, out string productId))
				Debug.LogError($"Can't find ID for product: {iapProduct}");
			else if (!TryGetProduct(productId, out product))
				Debug.LogError($"No product in catalog with id: {productId} ({iapProduct}).");
			else
				return true;

			return false;
		}


		bool TryGetProduct(string productId, out Product product)
		{
			product = default;
			bool hasProduct     = _iapCoreFacade.HasProductInCatalog( productId );

			try
			{
				product = hasProduct ? _iapCoreFacade.GetProductFromCatalog(productId) : null;
				return hasProduct && product != null;
			}
			catch
			{
				return false;
			}
		}


		int RoundPayoutQuantityToInt(Product product) => Mathf.RoundToInt(( float ) product.definition.payout.quantity);
	}
}