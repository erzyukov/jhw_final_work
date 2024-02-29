namespace Game.Configs
{
	using Sirenix.OdinInspector;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Localization;

	[CreateAssetMenu(fileName = "Localization", menuName = "Configs/Localization", order = (int)Config.Localization)]
	public class LocalizationConfig : SerializedScriptableObject
	{
		[SerializeField] private Locale _defaultLocale;
		[SerializeField] private string _stringTableKey;

		[SerializeField] private Dictionary<string, Locale> _locales = new();

		public Locale DefaultLocale => _defaultLocale;
		public string StringTableKey => _stringTableKey;

		public Locale GetLocale( string key )
		{
			_locales.TryGetValue(key, out Locale locale) ;

			return locale;
		}
	}
}