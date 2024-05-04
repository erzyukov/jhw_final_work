namespace Game.Profiles
{
	using Game.Units;
	using System;
	using System.Collections.Generic;
	using UniRx;

	[Serializable]
	public class GameProfile
    {
		public readonly IntReactiveProperty		LevelNumber			= new(1);
		public readonly IntReactiveProperty		WaveNumber			= new(0);
		public readonly IntReactiveProperty		SoftCurrency		= new(0);
		public readonly IntReactiveProperty		SummonCurrency		= new(0);
		public readonly IntReactiveProperty		HeroLevel			= new(1);
		public readonly IntReactiveProperty		HeroExperience		= new(0);

		public readonly IntReactiveProperty		LevelSoftCurrency		= new(0);
		public readonly IntReactiveProperty		LevelHeroExperience		= new(0);
		
		public bool			IsReturnFromBattle;
		public bool			IsWonLastBattle;
		public int			ReviveAttemptsCount;

		public List<LevelProfile>	Levels		= new();
		public TutorialProfile		Tutorial	= new();
		public HeroFieldProfile		HeroField	= new();
		public UnitsProfile			Units		= new();
		public EnergyProfile		Energy		= new();
		public List<Species>		Squad		= new();

		public readonly BoolReactiveProperty	IsMusicEnabled		= new(true);
		public readonly BoolReactiveProperty	IsSoundEnabled		= new(true);

		public AnalyticsProfile			Analytics		= new();

		public IapShopProfile			IapShopProfile	= new();
	}
}