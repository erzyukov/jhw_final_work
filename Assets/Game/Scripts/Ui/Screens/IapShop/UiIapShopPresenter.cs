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
		[Inject] private IUiIapShopScreen _screen;
		[Inject] private GameProfile _gameProfile;
		[Inject] private IIapFacade _iapFacade;

		IapShopProfile Profile => _gameProfile.IapShopProfile;

		public void Initialize()
		{
			_screen.Products.ForEach( product => InitProductElement( product ) );

			_iapFacade.OnBoughtOrRestored
				.Subscribe( OnProductBought )
				.AddTo( this );
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
				product.SetPrice( _iapFacade.GetLocalizedPrice( product.IapProduct ) );
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
