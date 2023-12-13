namespace Game.Profiles
{
	using System;
	using UniRx;

	[Serializable]
	public class LevelProfile
	{
		public BoolReactiveProperty Unlocked = new BoolReactiveProperty();

		public LevelProfile(bool unlocked)
		{
			Unlocked.Value = unlocked;
		}
	}
}