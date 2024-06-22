namespace Game.Ui
{
	using Game.Profiles;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Iap;
	using System.Linq;

	public class UiIapShopPresenter : ControllerBase, IInitializable
	{
		[Inject] private IUiIapShopScreen	_screen;
		[Inject] private GameProfile		_gameProfile;
		[Inject] private IIapFacade			_iapFacade;
		[Inject] private IIapConfig			_iapConfig;

		IapShopProfile Profile => _gameProfile.IapShopProfile;

		public void Initialize()
		{
			_iapFacade.IsInitialized
				.Where( v => v )
				.Subscribe( _ => IapInitializedHandler() )
				.AddTo( this );

			_iapFacade.OnBoughtOrRestored
				.Select( product => _iapConfig.BundleToId( product.definition.id ) )
				.Subscribe( OnProductBought )
				.AddTo( this );
		}

        void IapInitializedHandler()
        {
			_screen.Products.ForEach( product => InitProductElement( product ) );
		}

		private void OnProductBought( EIapProduct product )
		{
			var productElement = _screen.Products.Where( e => e.IapProduct == product ).FirstOrDefault();

			if (productElement != null)
				productElement.SetBought();
		}

		private void InitProductElement( UiIapShopElement product )
		{
			if (Profile.BoughtProducts.Contains( product.IapProduct ))
			{
				product.SetBought();
			}
			else
			{
				product.SetPrice( _iapFacade.GetLocalizedPriceString( product.IapProduct ) );
				product.Clicked
					.Subscribe( _ => OnProductBuy(product.IapProduct))
					.AddTo( this );
			}
		}

		private void OnProductBuy( EIapProduct iapProduct )
		{
			_iapFacade.Buy( iapProduct );
		}
	}
}
