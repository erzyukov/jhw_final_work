namespace Game.Core
{
	using Game.Gameplay;
	using Game.Units;
	using UniRx;

	public interface IGameplayEvents
	{
		ReactiveCommand<IUnitFacade> UnitsMerged { get; }
		ReactiveCommand<IUnitFacade> UnitSummoned { get; }
		ReactiveCommand<BattlefieldData> BattleStarted { get; }
		ReactiveCommand<BattlefieldData> BattleWon { get; }
		ReactiveCommand<BattlefieldData> BattleLost { get; }
	}

	public class GameplayEvents : IGameplayEvents
	{
		#region IGameplayEvents

		public ReactiveCommand<IUnitFacade> UnitsMerged { get; } = new ReactiveCommand<IUnitFacade>();
		public ReactiveCommand<IUnitFacade> UnitSummoned { get; } = new ReactiveCommand<IUnitFacade>();
		public ReactiveCommand<BattlefieldData> BattleStarted { get; } = new ReactiveCommand<BattlefieldData>();
		public ReactiveCommand<BattlefieldData> BattleWon { get; } = new ReactiveCommand<BattlefieldData>();
		public ReactiveCommand<BattlefieldData> BattleLost { get; } = new ReactiveCommand<BattlefieldData>();

		#endregion
	}
}
