namespace Game.Iap
{
	using Game.Profiles;
	using System;
	using Zenject;
	using UniRx;
	using UnityEngine.Purchasing;


	public class IapDeliver : IInitializable, IDisposable
	{
		[Inject] private IIapCoreFacade             _iapCoreFacade;
		[Inject] private IGameProfileManager        _gameProfileManager;
		[Inject] private IIapConfig                 _iapConfig;

		private readonly CompositeDisposable _lifetimeDisposables = new CompositeDisposable();

		private IapShopProfile		IapShopProfile => _gameProfileManager.GameProfile.IapShopProfile;

		public void Initialize()
		{
			_iapCoreFacade.OnBoughtOrRestored
				.Subscribe( OnBoughtOrRestored )
				.AddTo( _lifetimeDisposables );
		}

		public void Dispose() => _lifetimeDisposables.Clear();

		private void OnBoughtOrRestored( Product product )
		{
			EIapProduct type		= _iapConfig.BundleToId( product.definition.id );

			if (IapShopProfile.BoughtProducts.Contains( type ))
				return;

			if (GetProduct( type, product.definition.type ))
				_gameProfileManager.Save( true );
		}

		private bool GetProduct( EIapProduct type, ProductType productType )
		{
			if (productType == ProductType.NonConsumable)
				IapShopProfile.BoughtProducts.Add( type );

			switch (type)
			{
				case EIapProduct.NoAds:
					GetNoAds();
					break;

				default:
					return false;
			}

			return true;
		}

		private void GetNoAds()
		{
			IapShopProfile.NoAdsProduct.Value = true;
		}
	}
}
