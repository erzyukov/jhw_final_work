namespace Game.Core
{
	using Game.Utilities;
	using UniRx;
	using Zenject;

	public interface IGameCycle
	{
		ReactiveProperty<GameState> State { get; }
		void SetState(GameState state);
	}

	public class GameCycle : ControllerBase, IGameCycle, IInitializable
	{
		[Inject] IScenesManager _scenesManager;

		public ReactiveProperty<GameState> State { get; } = new ReactiveProperty<GameState>();

		public void Initialize()
		{
			_scenesManager.MainLoaded
				.Subscribe(_ => SetState(GameState.Lobby))
				.AddTo(this);
		}

		public void SetState(GameState state) => State.Value = state;
	}

	public enum GameState
	{
		None,
		LoadingLobby,
		Lobby,
		LoadingLevel,
		TacticalStage,
		BattleStage,
		CompleteWave,
		LoadingWave,
		CompleteBattle,
	}
}
