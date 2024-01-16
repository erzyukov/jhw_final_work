namespace Game.Core
{
	using Game.Configs;
	using UnityEngine.Localization.Settings;
	//using UnityEngine.ResourceManagement.AsyncOperations;
	using Zenject;
	using System.Collections;
	using UnityEngine;

	public interface ILocalizator
	{
		IEnumerator Preload();
		string GetString(string key);
	}

	public class Localizator : ILocalizator
	{
		[Inject] LocalizationConfig _config;

		#region IInitializable

		public IEnumerator Preload()
		{
			yield return LocalizationSettings.InitializationOperation;
			yield return new WaitForFixedUpdate();
			/*
			var loadingOperation = LocalizationSettings.InitializationOperation;
			
			while (loadingOperation.IsDone == false)
			{
				yield return null;
			}

			if (loadingOperation.Status == AsyncOperationStatus.Failed)
			{
				throw new System.Exception("Can't load locale!");
			}
			*/
			Debug.Log($"------------- LocalizationSettings.InitializationOperation complete ({Time.time})");

			LocalizationSettings.SelectedLocale = _config.DefaultLocale;

			yield return LocalizationSettings.InitializationOperation;
			yield return new WaitForFixedUpdate();

			Debug.Log($"------------- LocalizationSettings.InitializationOperation complete ({Time.time})");
		}

		public string GetString(string key) => 
			LocalizationSettings.StringDatabase.GetLocalizedString(_config.StringTableKey, key);

		#endregion
	}
}
