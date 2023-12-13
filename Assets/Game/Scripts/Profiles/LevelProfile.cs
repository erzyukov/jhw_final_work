namespace Game.Profiles
{
	using System;
	using UniRx;

	[Serializable]
	public class LevelProfile
	{
		public readonly ReactiveProperty<bool> Unlocked = new ReactiveProperty<bool>(true);
	}
}