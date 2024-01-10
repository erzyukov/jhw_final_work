namespace Game.Units
{
	using UniRx;

	public interface IUnitData
	{
		IntReactiveProperty Power { get; }
		IntReactiveProperty SupposedPower { get; }
		int GradeIndex { get; }
		bool IsHero { get; }
		float RendererHeight { get; }
		void Init(int grade, bool isHero, float height);
	}

	public class UnitData : IUnitData
	{
		#region IUnitData

		public IntReactiveProperty Power { get; } = new IntReactiveProperty();

		public IntReactiveProperty SupposedPower { get; } = new IntReactiveProperty();

		public int GradeIndex { get; private set; }

		public bool IsHero { get; private set; }

		public float RendererHeight { get; private set; }

		public void Init(int grade, bool isHero, float height)
		{
			GradeIndex = grade;
			IsHero = isHero;
			RendererHeight = height;
		}

		#endregion
	}
}
