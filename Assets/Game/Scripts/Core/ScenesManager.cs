namespace Game.Core
{
	using Game.Configs;
	using Game.Utilities;
	using Game.Profiles;
	using System;
	using System.Collections;
	using System.Linq;
	using System.Threading.Tasks;
	using UniRx;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using Zenject;

	public interface IScenesManager
	{
		ReactiveCommand ResourceLoading { get; }
		ReactiveCommand MainLoading { get; }
		ReactiveCommand MainLoaded { get; }
		ReactiveCommand LevelLoaded { get; }
		ReactiveProperty<float> SceneLoadProgress { get; }

		void LoadLevel();
		void UnloadLevel();
		void ReloadGame(Action scenesUnloadedCalback);
	}

	public class ScenesManager : MonoBehaviour, IScenesManager
	{
		[Inject] private ScenesConfig _scenesConfig;
		[Inject] private LevelsConfig _levelsConfig;
		[Inject] private ILocalizator _localizator;
		[Inject] private IGameProfileManager _profileManager;

		private void Start() => StartCoroutine(LoadScenes());

		private IEnumerator LoadScenes()
		{
			WaitForFixedUpdate wait = new WaitForFixedUpdate();

			ResourceLoading.Execute();

#if UNITY_EDITOR
			if (IsSceneLoaded(_scenesConfig.Main))
				throw new Exception("Load from Splash scene!");
#endif

			yield return _localizator.Preload();

			yield return new WaitUntil(() => _profileManager.IsReady.Value);

			yield return wait;

			MainLoading.Execute();

			if (!IsSceneLoaded(_scenesConfig.Main))
			{
				LoadScene(_scenesConfig.Main);

				yield return null;
			}

			yield return wait;

			MainLoaded.Execute();

			if (IsSceneLoaded(_scenesConfig.Splash))
				UnloadScene(_scenesConfig.Splash);

			yield return null;
		}

		#region IScenesManager

		public ReactiveCommand ResourceLoading { get; } = new ReactiveCommand();
		public ReactiveCommand MainLoading { get; } = new ReactiveCommand();
		public ReactiveCommand MainLoaded { get; } = new ReactiveCommand();
		public ReactiveCommand LevelLoaded { get; } = new ReactiveCommand();

		public ReactiveProperty<float> SceneLoadProgress { get; } = new ReactiveProperty<float>();

		public void LoadLevel()
		{
			LoadSceneAsync(GetLevelSceneName(), () =>
			{
				LevelLoaded.Execute();
				SetIslandActiveScene();
			});
		}

		public void UnloadLevel() => UnloadScene(GetLevelSceneName());

		public void ReloadGame(Action scenesUnloadedCalback)
		{
			StartCoroutine(ReloadScenes(scenesUnloadedCalback));
		}

		#endregion

		private IEnumerator ReloadScenes(Action scenesUnloadedCalback)
		{
			if (!IsSceneLoaded(_scenesConfig.Splash))
			{
				LoadScene(_scenesConfig.Splash);

				yield return null;
			}

			SceneManager.SetActiveScene(SceneManager.GetSceneByName(_scenesConfig.Splash));

			scenesUnloadedCalback.Invoke();

			if (IsSceneLoaded(GetLevelSceneName()))
				UnloadScene(GetLevelSceneName());

			yield return null;

			if (IsSceneLoaded(_scenesConfig.Main))
				UnloadScene(_scenesConfig.Main);

			yield return null;

			yield return LoadScenes();
		}

		private void SetIslandActiveScene() => SceneManager.SetActiveScene(SceneManager.GetSceneByName(GetLevelSceneName()));
		private bool IsSceneLoaded(string sceneName) => SceneManager.GetSceneByName(sceneName).isLoaded;
		private void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
		private void UnloadScene(string sceneName) => SceneManager.UnloadSceneAsync(sceneName);
		private string GetLevelSceneName()
		{
			Region region = _levelsConfig.Levels[_profileManager.GameProfile.LevelNumber.Value - 1].Region;

			return GetLevelSceneName(region);
		}

		private string GetLevelSceneName(Region regionType)
		{
			SceneField sceneField = _scenesConfig.Regions.Where(region => region.Region == regionType).FirstOrDefault().Scene;

			if (sceneField == null)
				throw new Exception($"Can`t found region scene with type: \"{regionType}\" in scenes config!");

			return sceneField.SceneName;
		}

		private async void LoadSceneAsync(string name, Action onLoadSceneCallback = null)
		{
			SceneLoadProgress.Value = 0;
			var loadingOperation = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
			
			await LoadingWaitAsync(loadingOperation, () => onLoadSceneCallback?.Invoke());
		}

		private async Task LoadingWaitAsync(AsyncOperation loadingOperation, Action onLoad)
		{
			while (!loadingOperation.isDone)
			{
				SceneLoadProgress.Value = loadingOperation.progress;

				await Task.Yield();
			}

			await Task.Yield();

			onLoad.Invoke();
		}
	}
}
