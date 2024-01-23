namespace Game.Core
{
	using Game.Dev;
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

		public ReactiveProperty<GameState> State { get; } = new ReactiveProperty<GameState>(GameState.None);

		public void Initialize()
		{
			_scenesManager.MainLoading
				.Subscribe(_ => SetState(GameState.LoadingLobby))
				.AddTo(this);

			_scenesManager.MainLoaded
				.Where(_ => State.Value == GameState.LoadingLobby)
				.Subscribe(_ => SetState(GameState.Lobby))
				.AddTo(this);

			State
				.Subscribe(v => WebGLDebug.Log($">>> StateChanged: {v}"))
				.AddTo(this);
		}

		public void SetState(GameState state) => State.Value = state;
	}

	public enum GameState
	{
		None			= 0,
		LoadingLobby,
		Lobby,
		LoadingLevel,
		TacticalStage,
		BattleStage,
		CompleteWave,
		LoadingWave,
		WinBattle,
		LoseBattle,
		HeroLevelReward,
		Upgrades,
		IapShop,
		Talents,
		GuildLobby,
	}
}
