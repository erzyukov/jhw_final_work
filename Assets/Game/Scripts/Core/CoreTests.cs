namespace Game.Core
{
	using Game.Profiles;
	using Game.Utilities;
	using Game.Dev;
	using UnityEngine;
	using UniRx;
	using Zenject;

	public class CoreTests : ControllerBase, IInitializable
    {
		[Inject] GameProfile _profile;
		[Inject] IScenesManager _scenesManager;

		public void Initialize()
		{
			ProfileSubscribe();
			SceneManagerSubscribe();
		}

		private void SceneManagerSubscribe()
		{
			_scenesManager.LevelLoaded
				.Subscribe(_ => WebGLDebug.Log($"[Test] ScenesManager: Level Loaded"))
				.AddTo(this);

			_scenesManager.SceneLoadProgress
				.Subscribe(v => WebGLDebug.Log($"[Test] ScenesManager: LoadingProgress={v}"))
				.AddTo(this);
		}

		private void ProfileSubscribe()
		{
			_profile.LevelNumber
				.Subscribe(v => WebGLDebug.Log($"[Test] LevelNumber changed: {v}"))
				.AddTo(this);

			_profile.WaveNumber
				.Subscribe(v => WebGLDebug.Log($"[Test] WaveNumber changed: {v}"))
				.AddTo(this);
		}
	}
}