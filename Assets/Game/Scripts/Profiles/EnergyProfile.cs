namespace Game.Profiles
{
	using System;
	using UniRx;

	[Serializable]
	public class EnergyProfile
	{
		public readonly IntReactiveProperty Amount = new IntReactiveProperty();
		public DateTime LastEnergyChange = DateTime.Now;
	}
}