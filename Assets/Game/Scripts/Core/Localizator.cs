namespace Game.Core
{
	using Game.Configs;
	using UnityEngine.Localization.Settings;
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
			WaitForFixedUpdate wait = new WaitForFixedUpdate();

			yield return LocalizationSettings.InitializationOperation;
			yield return wait;

			LocalizationSettings.SelectedLocale = _config.DefaultLocale;

			yield return LocalizationSettings.InitializationOperation;
			yield return wait;
		}

		public string GetString(string key) => 
			LocalizationSettings.StringDatabase.GetLocalizedString(_config.StringTableKey, key);

		#endregion
	}
}
