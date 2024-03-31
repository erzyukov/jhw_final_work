namespace Game.Profiles
{
	using System;
	using System.Collections.Generic;
	using UniRx;

	[Serializable]
	public class GameProfile
    {
		public readonly IntReactiveProperty LevelNumber = new IntReactiveProperty(1);
		public readonly IntReactiveProperty WaveNumber = new IntReactiveProperty(0);
		public readonly IntReactiveProperty SoftCurrency = new IntReactiveProperty(0);
		public readonly IntReactiveProperty SummonCurrency = new IntReactiveProperty(0);
		public readonly IntReactiveProperty HeroLevel = new IntReactiveProperty(1);
		public readonly IntReactiveProperty HeroExperience = new IntReactiveProperty(0);

		public readonly IntReactiveProperty LevelSoftCurrency = new IntReactiveProperty(0);
		public readonly IntReactiveProperty LevelHeroExperience = new IntReactiveProperty(0);
		public bool IsReturnFromBattle;
		public bool IsWonLastBattle;
		public int ReviveAttemptsCount;

		public List<LevelProfile> Levels = new List<LevelProfile>();
		public TutorialProfile Tutorial = new TutorialProfile();
		public HeroFieldProfile HeroField = new HeroFieldProfile();
		public UnitsProfile Units = new UnitsProfile();
		public EnergyProfile Energy = new EnergyProfile();

		public readonly BoolReactiveProperty IsMusicEnabled = new BoolReactiveProperty(true);
		public readonly BoolReactiveProperty IsSoundEnabled = new BoolReactiveProperty(true);

		public AnalyticsProfile Analytics = new AnalyticsProfile();

		public IapShopProfile IapShopProfile = new IapShopProfile();
	}
}