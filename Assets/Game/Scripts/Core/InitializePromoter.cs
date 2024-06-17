namespace Game.Core
{
	using UniRx;

	public interface IInitializePromoter
	{
		BoolReactiveProperty IsReadyToLevelLoad { get; }
		void SetAdsAsReady();
	}

	public class InitializePromoter : IInitializePromoter
	{

#region IInitializePromoter

		public BoolReactiveProperty IsReadyToLevelLoad		{ get; } = new();

		public void SetAdsAsReady()
		{
			IsReadyToLevelLoad.Value = true;
		}

#endregion

	}
}