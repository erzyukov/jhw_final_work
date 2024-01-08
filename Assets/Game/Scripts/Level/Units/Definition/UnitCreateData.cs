namespace Game.Units
{
	using System;

	[Serializable]
	public struct UnitCreateData
	{
		public Species Species;
		public int GradeIndex;
		public int Power;
		public bool IsHero;
	}
}