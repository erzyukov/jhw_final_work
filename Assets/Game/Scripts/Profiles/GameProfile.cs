namespace Game.Profiles
{
	using System;
	using UniRx;

	[Serializable]
	public class GameProfile
    {
		public IntReactiveProperty RegionNumber = new IntReactiveProperty(1);
		public IntReactiveProperty LevelNumber = new IntReactiveProperty(1);
		public IntReactiveProperty WaveNumber = new IntReactiveProperty(1);
	}
}
