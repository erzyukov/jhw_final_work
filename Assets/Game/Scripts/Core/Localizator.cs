namespace Game.Core
{
	using Game.Configs;
	using Game.Utilities;
	using UnityEngine.Localization.Settings;
	using Zenject;

	public interface ILocalizator
	{
		string GetString(string key);
	}

	public class Localizator : ControllerBase, ILocalizator, IInitializable
	{
		[Inject] LocalizationConfig _config;

		public void Initialize()
		{
			LocalizationSettings.SelectedLocale = _config.DefaultLocale;
		}

		#region IInitializable

		public string GetString(string key) => 
			LocalizationSettings.StringDatabase.GetLocalizedString(_config.StringTableKey, key);

		#endregion
	}
}
