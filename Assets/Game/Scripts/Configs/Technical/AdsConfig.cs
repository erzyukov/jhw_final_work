namespace Game.Configs
{
	using Sirenix.OdinInspector;
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu( fileName = "Ads", menuName = "Configs/Ads", order = (int)Config.Ads )]
	public class AdsConfig : SerializedScriptableObject
	{
		public List<EAdsBlocker> BlockersInter;
		public List<EAdsBlocker> BlockersBanner;

		public float InterstitialInterval;

		public int InterActiveLevelNumber;

		public string MaxSdkKey;
	}
}
