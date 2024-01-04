namespace Game.Profiles
{
	using System;
	using System.Collections.Generic;
	using Game.Units;

	[Serializable]
	public class HeroFieldProfile
	{
		public List<Unit> Units = new List<Unit>();

		[Serializable]
		public struct Unit
		{
			public Species Species;
			public int GradeIndex;
			public int Power;
			public SVector2Int Position;
		}
	}
}