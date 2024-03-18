namespace Game.Core
{
	using Game.Gameplay;
	using Game.Units;
	using UniRx;

	public interface IResourceEvents
	{
		ReactiveCommand LowHeroLevelAlert { get; }
	}

	public class ResourceEvents : IResourceEvents
	{
		#region IResourceEvents

		public ReactiveCommand LowHeroLevelAlert { get; } = new();

		#endregion
	}
}
