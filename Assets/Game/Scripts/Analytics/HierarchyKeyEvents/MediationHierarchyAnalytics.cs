namespace Game.Analytics
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Managers;

	public class MediationHierarchyAnalytics : HierarchyAnalyticsBase, IInitializable
	{
		[Inject] protected IAdsProvider _adsProvider;

		private const string MediationEventKey = "Ad";

		private const string LoadedValue = "available";
		private const string OpenedValue = "started";
		private const string ClosedValue = "watched";

		public void Initialize()
		{
			_adsProvider.AdLoaded
				.Subscribe( type => SendMediationEvent(type, LoadedValue) )
				.AddTo( this );

			_adsProvider.AdOpened
				.Subscribe( type => SendMediationEvent(type, OpenedValue) )
				.AddTo( this );

			_adsProvider.AdClosed
				.Subscribe( type => SendMediationEvent(type, ClosedValue) )
				.AddTo( this );
		}

		private void SendMediationEvent( EAdType type, string status )
		{
			string place = type switch {
				EAdType.Interstitial => _adsProvider.InterstitialPlace,
				EAdType.RewardedVideo =>  _adsProvider.RewardedPlace,
				EAdType.Banner => _adsProvider.BannerPlace,
				_ => "unknown",
			};

			//SendDesignEvent($"{MediationEventKey}:{status}:{type}:{place}");
		}
	}
}