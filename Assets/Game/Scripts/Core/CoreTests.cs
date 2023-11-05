namespace Game.Core
{
	using Game.Profiles;
	using Game.Utilities;
	using UnityEngine;
	using VContainer;
	using VContainer.Unity;
	using UniRx;

	public class CoreTests : ControllerBase, IStartable
    {
		[Inject] GameProfile _profile;
		[Inject] IScenesManager _scenesManager;

		public void Start()
		{
			ProfileSubscribe();
			SceneManagerSubscribe();
		}

		private void SceneManagerSubscribe()
		{
			_scenesManager.LevelLoaded
				.Subscribe(_ => Debug.Log($"[Test] ScenesManager: Level Loaded"))
				.AddTo(this);

			_scenesManager.SceneLoadProgress
				.Subscribe(v => Debug.Log($"[Test] ScenesManager: LoadingProgress={v}"))
				.AddTo(this);
		}

		private void ProfileSubscribe()
		{
			_profile.LevelNumber
				.Subscribe(v => Debug.Log($"[Test] LevelNumber changed: {v}"))
				.AddTo(this);

			_profile.WaveNumber
				.Subscribe(v => Debug.Log($"[Test] WaveNumber changed: {v}"))
				.AddTo(this);
		}
	}
}