using UniRx;

namespace Game.Units
{
	public interface IUnitData
	{
		FloatReactiveProperty Power { get; }
		float RendererHeight { get; }
		void SetRendererHeight(float value);
	}

	public class UnitData : IUnitData
	{
		#region IUnitData

		public FloatReactiveProperty Power { get; } = new FloatReactiveProperty();

		public float RendererHeight { get; private set; }

		public void SetRendererHeight(float value) =>
			RendererHeight = value;

		#endregion
	}
}
