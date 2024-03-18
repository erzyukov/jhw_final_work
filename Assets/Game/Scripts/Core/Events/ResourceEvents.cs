namespace Game.Core
{
	using UniRx;

	public interface IResourceEvents
	{
		ReactiveCommand LowHeroLevelAlert { get; }
		ReactiveCommand LowEnergyAlert { get; }
	}

	public class ResourceEvents : IResourceEvents
	{
		#region IResourceEvents

		public ReactiveCommand LowHeroLevelAlert { get; } = new();
		public ReactiveCommand LowEnergyAlert { get; } = new();

		#endregion
	}
}
