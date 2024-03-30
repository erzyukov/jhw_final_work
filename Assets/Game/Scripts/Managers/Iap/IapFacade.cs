namespace Game.Iap
{
	using Game.Utilities;
	using Zenject;
	using UniRx;

	public struct PurchaseData
	{
		public string   Currency;
		public string   ItemId;
		public string   Receipt;
	}

	public interface IIapFacade
	{
		BoolReactiveProperty IsInitialized { get; }
		ReactiveCommand<EIapProduct> OnBoughtOrRestored { get; }
		ReactiveCommand<EIapProduct> OnPurchaseFailed { get; }
		void Buy(EIapProduct product);
		string GetLocalizedPrice( EIapProduct product );
	}

	public class IapFacade : ControllerBase, IIapFacade, IInitializable
	{
		[Inject] private IIapCoreFacade _iapCoreFacade;

		public void Initialize() {}

		#region IIapFacade

		public BoolReactiveProperty IsInitialized => _iapCoreFacade.IsInitialized;

		public ReactiveCommand<EIapProduct> OnBoughtOrRestored => _iapCoreFacade.OnBoughtOrRestored;

		public ReactiveCommand<EIapProduct> OnPurchaseFailed => _iapCoreFacade.OnPurchaseFailed;

		public void Buy(EIapProduct product) => _iapCoreFacade.Buy(product);

		public string GetLocalizedPrice( EIapProduct product ) => _iapCoreFacade.GetLocalizedPrice( product );

		#endregion
	}
}
