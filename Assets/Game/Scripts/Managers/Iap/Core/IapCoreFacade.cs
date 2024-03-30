namespace Game.Iap
{
	using System;
	using UniRx;
	using Zenject;

	public interface IIapCoreFacade
	{
		BoolReactiveProperty IsInitialized { get; }
		ReactiveCommand<EIapProduct> OnBoughtOrRestored { get; }
		ReactiveCommand<EIapProduct> OnPurchaseFailed { get; }

		void Buy( EIapProduct product );
		void Restore();

		bool IsBought( EIapProduct productType );

		string GetLocalizedPrice( EIapProduct product );
	}

	public class IapCoreFacade : IIapCoreFacade
	{

		[Inject] IIapCore _iapCore;

		public BoolReactiveProperty IsInitialized { get; } = new();

		public ReactiveCommand<EIapProduct> OnBoughtOrRestored { get; } = new();

		public ReactiveCommand<EIapProduct> OnPurchaseFailed { get; } = new();

		public void Buy( EIapProduct product ) => 
			_iapCore.Buy( product );

		public bool IsBought( EIapProduct productType )
		{
			throw new NotImplementedException();
		}

		public void Restore()
		{
			throw new NotImplementedException();
		}

		public string GetLocalizedPrice( EIapProduct product ) => _iapCore.GetLocalizedPrice( product );
	}
}
