namespace Game.Units
{
	using System;

	// TODO: refact: use this struct only for build unit, otherwise use UnitData

	[Serializable]
	public struct UnitCreateData
	{
		public Species Species;
		public int GradeIndex;
		public int Power;
		public bool IsHero;
	}
}