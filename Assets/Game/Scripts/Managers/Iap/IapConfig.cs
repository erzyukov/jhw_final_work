namespace Game.Iap
{
	using Game.Configs;
	using UnityEngine;

	public interface IIapConfig: IIapCoreConfig {}

	[CreateAssetMenu(fileName = "IAP", menuName = "Configs/IAP", order = ( int ) Config.Iap)]
	public class IapConfig : IapCoreConfig, IIapConfig {}
}
