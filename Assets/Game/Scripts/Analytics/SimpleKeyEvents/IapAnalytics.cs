namespace Game.Analytics
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Iap;
	using System.Collections.Generic;
	using Newtonsoft.Json;
	using UnityEngine;

	public class IapAnalytics : AnalyticsBase, IInitializable
	{
		[Inject] protected IIapFacade _iapFacade;

		private const string PurchaseEvent			= "payment_succeed";

		public void Initialize()
		{
			_iapFacade.OnValidatedPurchase
				.Subscribe( SendRevenue )
				.AddTo( this );
		}

		private void SendRevenue( PurchaseData data )
		{
#if UNITY_ANDROID
			var recptToJSON		= JsonConvert.DeserializeObject<Dictionary<string, object>>(data.Receipt);
			var receiptPayload	= JsonConvert.DeserializeObject<Dictionary<string, object>>((string)recptToJSON["Payload"]);
			var signature		= (string)receiptPayload["signature"];
			var transactionID	= (string)recptToJSON["TransactionID"];

			IapRevenueData revenue = new(){
				PriceMicros				= data.CentsAmount * 10000,
				Currency				= data.Currency,
				Payload					= (string)recptToJSON["Payload"],
				ProductID				= data.ItemId,
				Quantity				= 1,
				ReceiptData				= data.Receipt,
				ReceiptSignature		= signature,
				ReceiptTransactionID	= transactionID,
			};

			SendIapRevenue( revenue );

			var properties = new Dictionary<string, object>
			{
				{ "inapp_id",	data.ItemId },
				{ "price",		data.CentsAmount },
				{ "inapp_type",	data.ItemType },
				{ "place",		data.CartType },
			};

			SendMessage( PurchaseEvent, properties );
#endif
		}
	}
}
