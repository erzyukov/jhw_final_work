namespace Game.Core
{
	using UniRx;

	public interface IGameplayEvents
	{
		ReactiveCommand UnitsMerged { get; }
		ReactiveCommand<int> UnitSummoned { get; }
	}

	public class GameplayEvents : IGameplayEvents
	{
		#region IGameplayEvents

		public ReactiveCommand UnitsMerged { get; } = new ReactiveCommand();
		public ReactiveCommand<int> UnitSummoned { get; } = new ReactiveCommand<int>();

		#endregion
	}
}
