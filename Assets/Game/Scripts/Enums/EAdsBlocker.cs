namespace Game
{
	public enum EAdsBlocker
	{
		None = 0,

		Mediation_Loading = 1,
		GDPR,

		NoAds_IAP = 50,

		UI_IAP = 100,
		UI_Shop,
		UI_Offer,
		UI_RateUs,
		UI_Loading,
		UI_Info, // Win, Lose, LevelReward

		DevMenu,

		Banner_LevelNotLoaded,
	}
}
