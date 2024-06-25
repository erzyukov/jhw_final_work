namespace Game.Core
{
	using Game.Configs;
	using UnityEngine.Localization.Settings;
	using Zenject;
	using System.Collections;
	using UnityEngine;
	using UnityEngine.Localization;
	using UniRx;
	using System.Globalization;


	public interface ILocalizator
	{
		StringReactiveProperty LangKey { get; }
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

		public StringReactiveProperty LangKey { get; } = new();

		public IEnumerator Preload()
		{
			yield return LocalizationSettings.InitializationOperation;

			_locale = (Application.systemLanguage != SystemLanguage.Unknown)
				? LocalizationSettings.AvailableLocales.GetLocale( Application.systemLanguage )
				: LocalizationSettings.AvailableLocales.GetLocale( CultureInfo.CurrentCulture );

			LocalizationSettings.SelectedLocale		= _locale ?? _config.DefaultLocale;

			LangKey.Value		= _locale.Identifier.Code;

			yield return LocalizationSettings.InitializationOperation;
		}

		public void SetLocale( string key )
		{
			LangKey.Value = key;
			_locale = _config.GetLocale( key );
		}

		public string GetString( string key ) =>
			LocalizationSettings.StringDatabase.GetLocalizedString( _config.StringTableKey, key );

		public Sprite GetSprite( string key ) =>
			LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>( _config.AssetTableKey, key );

		#endregion
	}
}