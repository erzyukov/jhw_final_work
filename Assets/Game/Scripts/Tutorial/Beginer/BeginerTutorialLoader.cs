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
			if (IsTutorialComplete)
				return;

			_cycle.State
				.Where(state => state == GameState.LoadingLobby && IsTutorialComplete == false)
				.Subscribe(_ => OnLoadingLobbyHandler())
				.AddTo(this);

			_profile.Tutorial.BeginnerStep
				.Where(_ => IsTutorialComplete)
				.Subscribe(_ => Dispose())
				.AddTo(this);
		}

		private void OnLoadingLobbyHandler()
		{
			BeginnerStep currentStep = (_profile.Tutorial.BeginnerStep.Value == BeginnerStep.None) ? BeginnerStep.FirstSummon : _profile.Tutorial.BeginnerStep.Value;

			_level.LevelLoading
				.Where(v => currentStep != BeginnerStep.Complete)
				.Subscribe(_ => _profile.Tutorial.BeginnerStep.SetValueAndForceNotify(currentStep))
				.AddTo(this);

			_level.GoToLevel(_profile.LevelNumber.Value);
		}

		private bool IsTutorialComplete => _profile.Tutorial.BeginnerStep.Value == BeginnerStep.Complete;
	}
}