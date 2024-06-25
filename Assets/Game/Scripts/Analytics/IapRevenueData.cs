using static Io.AppMetrica.Revenue;

namespace Game.Analytics
{
	public struct IapRevenueData
	{
		public long		PriceMicros;
		public string	Currency;
		public string	Payload;
        public string	ProductID;
        public int?		Quantity;
        public string	ReceiptData;
        public string	ReceiptSignature;
        public string	ReceiptTransactionID;
	}
}
