namespace Game.Iap
{
	using Game.Profiles;
	using System;
	using Zenject;
	using UniRx;

	public class IapDeliver : IInitializable, IDisposable
	{
		[Inject] private IIapCoreFacade             _iapCoreFacade;
		[Inject] private IGameProfileManager        _gameProfileManager;
		[Inject] private IIapConfig                 _iapConfig;

		private readonly CompositeDisposable _lifetimeDisposables = new CompositeDisposable();

		IapShopProfile IapShopProfile => _gameProfileManager.GameProfile.IapShopProfile;

		public void Initialize()
		{
			_iapCoreFacade.OnBoughtOrRestored
				.Subscribe( OnBoughtOrRestored )
				.AddTo( _lifetimeDisposables );
		}

		public void Dispose() => _lifetimeDisposables.Clear();

		private void OnBoughtOrRestored( EIapProduct product )
		{
			if (IapShopProfile.BoughtProducts.Contains( product ))
				return;

			if (GetProduct( product ))
				_gameProfileManager.Save( true );
		}

		private bool GetProduct( EIapProduct product )
		{
			if (_iapConfig.TryGetIapData( product, out IapData data ) == false)
				return false;

			if (data.IsConsumable == false)
				IapShopProfile.BoughtProducts.Add( product );

			switch (product)
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
