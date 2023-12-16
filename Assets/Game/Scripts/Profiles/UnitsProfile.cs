namespace Game.Profiles
{
	using System;
	using System.Collections.Generic;
	using Game.Units;
	using UniRx;

	[Serializable]
	public class UnitsProfile
	{
		public Dictionary<Species, IntReactiveProperty> Upgrades = new Dictionary<Species, IntReactiveProperty>();
	}
}