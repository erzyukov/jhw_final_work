using UniRx;

namespace Game.Units
{
	public interface IUnitData
	{
		IntReactiveProperty Power { get; }
		float RendererHeight { get; }
		void SetRendererHeight(float value);
	}

	public class UnitData : IUnitData
	{
		#region IUnitData

		public IntReactiveProperty Power { get; } = new IntReactiveProperty();

		public float RendererHeight { get; private set; }

		public void SetRendererHeight(float value) =>
			RendererHeight = value;

		#endregion
	}
}
