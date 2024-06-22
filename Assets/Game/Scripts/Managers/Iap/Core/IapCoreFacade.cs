namespace Game.Iap
{
	using UniRx;
	using UnityEngine.Purchasing;
	using UnityEngine.Purchasing.Extension;
	using Zenject;


	public interface IIapCoreFacade
	{
		BoolReactiveProperty				IsInitialized				{get;}
		ReactiveCommand<bool>				OnRestoreProcessFinish		{get;}
		ReactiveCommand<Product>			OnBoughtOrRestored			{get;}
		ReactiveCommand<EIapProduct>		OnPurchaseFailed			{get;}
		ReactiveCommand<PurchaseData>		OnValidatedPurchase			{get;}
		ReactiveCommand<PurchaseFailureDescription>		OnPurchaseFailedDetailed {get;}

		bool IsRestoreMode		{get;}
		bool Sandbox			{get; set;}
		bool NoValidation		{get; set;}

		void Buy( EIapProduct product );
		void Restore();

		bool HasProductInCatalog( string productId );
		Product GetProductFromCatalog( string productId );
		bool IsBought(EIapProduct productType);
		string GetLocalizedPrice( EIapProduct product );
	}


	public class IapCoreFacade: IIapCoreFacade
	{
#region External

		[Inject] IIapCoreListener	_iapCoreListener;
		[Inject] IIapCore			_iapCore;

#endregion
#region IIapCoreFacade

		public BoolReactiveProperty				IsInitialized				{get;} = new BoolReactiveProperty();
		public ReactiveCommand<bool>			OnRestoreProcessFinish		{get;} = new ReactiveCommand<bool>();
		public ReactiveCommand<Product>			OnBoughtOrRestored			{get;} = new ReactiveCommand<Product>();
		public ReactiveCommand<EIapProduct>		OnPurchaseFailed			{get;} = new ReactiveCommand<EIapProduct>();
		public ReactiveCommand<PurchaseData>	OnValidatedPurchase			{get;} = new ReactiveCommand<PurchaseData>();
		public ReactiveCommand<PurchaseFailureDescription> OnPurchaseFailedDetailed { get; } = new ReactiveCommand<PurchaseFailureDescription>();

		public bool IsRestoreMode		=> _iapCore.IsRestoreMode;
		public bool Sandbox				{get; set;}
		public bool NoValidation		{get; set;}

		public void Buy( EIapProduct product )			=> _iapCore.Buy( product );
		public void Restore()							=> _iapCore.Restore();
		public bool IsBought(EIapProduct productType)	=> _iapCore.IsBought(productType);

		public bool HasProductInCatalog( string productId )				=> _iapCoreListener.HasProductInCatalog( productId );
		public Product GetProductFromCatalog( string productId )		=> _iapCoreListener.GetProductFromCatalog( productId );
		public string GetLocalizedPrice( EIapProduct product ) => _iapCore.GetLocalizedPrice( product );
#endregion
	}
}