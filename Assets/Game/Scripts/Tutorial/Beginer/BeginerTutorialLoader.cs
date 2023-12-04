namespace Game.Tutorial
{
	using Game.Core;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Profiles;

	public class BeginerTutorialLoader : ControllerBase, IInitializable
	{
		[Inject] private GameProfile _profile;
		[Inject] private IGameCycle _cycle;
		[Inject] private IGameLevel _level;

		public void Initialize()
		{
			if (_profile.Tutorial.BeginerStep.Value == BeginnerStep.None)
			{
				_cycle.State
					.Where(state => state == GameState.LoadingLobby)
					.Subscribe(_ => OnLoadingLobbyHandler())
					.AddTo(this);
			}
		}

		private void OnLoadingLobbyHandler()
		{
			_level.GoToLevel(_profile.LevelNumber.Value);
			_level.IsLevelLoaded
				.Where(v => v && _profile.Tutorial.BeginerStep.Value != BeginnerStep.Complete)
				.Subscribe(_ => _profile.Tutorial.BeginerStep.Value = BeginnerStep.FirstSummon)
				.AddTo(this);
		}
	}
}