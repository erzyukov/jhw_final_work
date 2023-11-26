namespace Game.Profiles
{
	using System;
	using UniRx;

	[Serializable]
	public class GameProfile
    {
		public readonly IntReactiveProperty LevelNumber = new IntReactiveProperty(1);
		public readonly IntReactiveProperty WaveNumber = new IntReactiveProperty(0);
		public readonly IntReactiveProperty SoftCurrency = new IntReactiveProperty(0);
		public readonly IntReactiveProperty SummonCurrency = new IntReactiveProperty(0);
		public readonly IntReactiveProperty HeroLevel = new IntReactiveProperty(1);
		public readonly IntReactiveProperty HeroLevelExperience = new IntReactiveProperty(30);

	}
}
