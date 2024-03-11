namespace Game.Core
{
	using Game.Configs;
	using UnityEngine.Localization.Settings;
	using Zenject;
	using System.Collections;
	using UnityEngine;
	using UnityEngine.Localization;
	using UniRx;

	public interface ILocalizator
	{
		BoolReactiveProperty IsLangInitialized { get; }
		IEnumerator Preload();
		void SetLocale( string key );
		string GetString( string key );
		Sprite GetSprite( string key );
	}

	public class Localizator : ILocalizator
	{
		[Inject] LocalizationConfig _config;

		private Locale _locale;

		#region IInitializable

		public BoolReactiveProperty IsLangInitialized { get; } = new();

		public IEnumerator Preload()
		{
			WaitForFixedUpdate wait = new WaitForFixedUpdate();

			yield return LocalizationSettings.InitializationOperation;
			yield return wait;

			LocalizationSettings.SelectedLocale = _locale ?? _config.DefaultLocale;

			yield return LocalizationSettings.InitializationOperation;
			yield return wait;

			IsLangInitialized.Value = true;
		}

		public void SetLocale( string key ) =>
			_locale = _config.GetLocale( key );

		public string GetString( string key ) =>
			LocalizationSettings.StringDatabase.GetLocalizedString( _config.StringTableKey, key );

		public Sprite GetSprite( string key ) =>
			LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>( _config.AssetTableKey, key );

		#endregion
	}
}
