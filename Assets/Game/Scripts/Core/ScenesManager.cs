namespace Game.Core
{
	using Configs;
	using System;
	using System.Collections;
	using System.Threading.Tasks;
	using UniRx;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	using VContainer;

	public interface IScenesManager
	{
		ReactiveCommand OnSplashComplete { get; }
		ReactiveCommand OnLevelLoaded { get; }
		ReactiveProperty<float> SceneLoadProgress { get; }
	}

	public class ScenesManager : MonoBehaviour, IScenesManager
	{
		[Inject] private ScenesConfig _scenesConfig;

		private const int DefaultLevelIndex = 0;
		private void Start() => StartCoroutine(LoadScenes());

		private IEnumerator LoadScenes()
		{
			if (IsSceneLoaded(_scenesConfig.Splash))
			{
				yield return null;

				OnSplashComplete.Execute();
			}

			if (!IsSceneLoaded(_scenesConfig.Main))
			{
				LoadScene(_scenesConfig.Main);

				yield return null;
			}

			if (IsSceneLoaded(_scenesConfig.Splash))
				UnloadScene(_scenesConfig.Splash);

			LoadLevel();

			yield return null;
		}

		#region IScenesManager

		public ReactiveCommand OnSplashComplete { get; } = new ReactiveCommand();
		public ReactiveCommand OnLevelLoaded { get; } = new ReactiveCommand();

		public ReactiveProperty<float> SceneLoadProgress { get; } = new ReactiveProperty<float>();

		#endregion

		private void LoadLevel()
		{
			LoadSceneAsync(GetLevelSceneName(), () =>
			{
				OnLevelLoaded.Execute();
				SetIslandActiveScene();
			});
		}

		private void SetIslandActiveScene() => SceneManager.SetActiveScene(SceneManager.GetSceneByName(GetLevelSceneName()));
		private bool IsSceneLoaded(string sceneName) => SceneManager.GetSceneByName(sceneName).isLoaded;
		private void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
		private void UnloadScene(string sceneName) => SceneManager.UnloadSceneAsync(sceneName);
		private string GetLevelSceneName() => _scenesConfig.Levels[DefaultLevelIndex].SceneName;

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
