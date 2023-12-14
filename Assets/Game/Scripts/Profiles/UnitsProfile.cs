namespace Game.Profiles
{
	using System;
	using System.Collections.Generic;
	using Game.Units;

	[Serializable]
	public class UnitsProfile
	{
		public Dictionary<Species, int> Upgrades = new Dictionary<Species, int>();
	}
}