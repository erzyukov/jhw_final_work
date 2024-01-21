namespace Game.Core
{
	using Game.Gameplay;
	using UniRx;

	public interface IGameplayEvents
	{
		ReactiveCommand UnitsMerged { get; }
		ReactiveCommand<int> UnitSummoned { get; }
		ReactiveCommand<BattlefieldData> BattleStarted { get; }
		ReactiveCommand<BattlefieldData> BattleWon { get; }
		ReactiveCommand<BattlefieldData> BattleLost { get; }
	}

	public class GameplayEvents : IGameplayEvents
	{
		#region IGameplayEvents

		public ReactiveCommand UnitsMerged { get; } = new ReactiveCommand();
		public ReactiveCommand<int> UnitSummoned { get; } = new ReactiveCommand<int>();
		public ReactiveCommand<BattlefieldData> BattleStarted { get; } = new ReactiveCommand<BattlefieldData>();
		public ReactiveCommand<BattlefieldData> BattleWon { get; } = new ReactiveCommand<BattlefieldData>();
		public ReactiveCommand<BattlefieldData> BattleLost { get; } = new ReactiveCommand<BattlefieldData>();

		#endregion
	}
}
