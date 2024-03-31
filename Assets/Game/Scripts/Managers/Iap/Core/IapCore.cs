using Game.Profiles;
using Zenject;

namespace Game.Iap
{

	public interface IIapCore
	{
		void Buy( EIapProduct product );
		void Restore();
		bool IsBought( EIapProduct product );
		string GetLocalizedPrice( EIapProduct product );
	}

	public class IapCore : IIapCore
	{
		[Inject] protected IIapConfig IapConfig;
		
		[Inject] private IGameProfileManager _gameProfileManager;

		protected IapShopProfile IapShopProfile => _gameProfileManager.GameProfile.IapShopProfile;

		#region IIapCore

		public virtual void Buy( EIapProduct product ) {}

		public void Restore() {}

		public virtual bool IsBought( EIapProduct product ) {return false;}

		public virtual string GetLocalizedPrice( EIapProduct product )
		{
			if (IapConfig.TryGetIapData( product, out var data ) == false)
				return string.Empty;

			return data.Price.ToString();
		}

		#endregion
	}
}
