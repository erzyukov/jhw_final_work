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
		public readonly IntReactiveProperty HeroExperience = new IntReactiveProperty(7);
        
        public readonly IntReactiveProperty Energy = new IntReactiveProperty();
        public DateTime LastEnergyChange = new DateTime();

        public readonly IntReactiveProperty LevelSoftCurrency = new IntReactiveProperty(0);
		public readonly IntReactiveProperty LevelHeroExperience = new IntReactiveProperty(0);
		public bool IsReturnFromBattle;

		public List<LevelProfile> Levels = new List<LevelProfile>();

		public TutorialProfile Tutorial = new TutorialProfile();
		public HeroFieldProfile HeroField = new HeroFieldProfile();
		public UnitsProfile Units = new UnitsProfile();
	}
}