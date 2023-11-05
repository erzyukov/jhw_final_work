namespace Game.Profiles
{
	using System;
	using UniRx;

	[Serializable]
	public class GameProfile
    {
		public readonly IntReactiveProperty LevelNumber = new IntReactiveProperty(1);
		public readonly IntReactiveProperty WaveNumber = new IntReactiveProperty(0);
	}
}
