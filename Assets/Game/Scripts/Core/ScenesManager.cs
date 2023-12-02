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
		ReactiveCommand SplashCompleted { get; }
		ReactiveCommand MainLoaded { get; }
		ReactiveCommand LevelLoaded { get; }
		ReactiveProperty<float> SceneLoadProgress { get; }

		void LoadLevel();
		void UnloadLevel();
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

			if (IsSceneLoaded(_scenesConfig.Splash))
			{
				yield return _localizator.Preload();

				yield return null;

				SplashCompleted.Execute();
			}

			yield return wait;

			if (!IsSceneLoaded(_scenesConfig.Main))
			{
				LoadScene(_scenesConfig.Main);

				yield return null;
			}

			yield return wait;

			MainLoaded.Execute();

			if (IsSceneLoaded(_scenesConfig.Splash))
				UnloadScene(_scenesConfig.Splash);

			//LoadLevel();

			yield return null;
		}

		#region IScenesManager

		public ReactiveCommand SplashCompleted { get; } = new ReactiveCommand();
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

		#endregion

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
