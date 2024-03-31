namespace Game.Iap
{
	using UniRx;
	using UnityEngine;
	using YG;

	public class IapCoreYandexListener : IapCoreListener
	{
		public override void Initialize()
		{
			Observable.FromEvent(
				x => YandexGame.GetPaymentsEvent += x,
				x => YandexGame.GetPaymentsEvent -= x
			)
				.Subscribe( _ => OnInitialized() )
				.AddTo( LifetimeDisposable );

			Observable.FromEvent<string>(
				x => YandexGame.PurchaseSuccessEvent += x,
				x => YandexGame.PurchaseSuccessEvent -= x
			)
				.Subscribe( OnPurchaseSuccess )
				.AddTo( LifetimeDisposable );

			Observable.FromEvent<string>(
				x => YandexGame.PurchaseFailedEvent += x,
				x => YandexGame.PurchaseFailedEvent -= x
			)
				.Subscribe( OnPurchaseFailed )
				.AddTo( LifetimeDisposable );
		}

		private void OnInitialized()
		{
			IapCoreFacade.IsInitialized.Value = true;
			
			YandexGame.ConsumePurchases();
		}

		private void OnPurchaseFailed( string bandleId )
		{
			EIapProduct productType = IapConfig.BundleToId( bandleId );
			IapCoreFacade.OnPurchaseFailed.Execute( productType );
		}

		private void OnPurchaseSuccess( string bandleId )
		{
			Debug.LogWarning($">>> OnPurchaseSuccess: {bandleId}");

			EIapProduct productType = IapConfig.BundleToId( bandleId );
			IapCoreFacade.OnBoughtOrRestored.Execute( productType );
		}
	}
}
