namespace Game.Configs
{
	using UnityEngine;
	using UnityEngine.Localization;

	[CreateAssetMenu(fileName = "Localization", menuName = "Configs/Localization", order = (int)Config.Localization)]
	public class LocalizationConfig : ScriptableObject
	{
		[SerializeField] private Locale _defaultLocale;
		[SerializeField] private string _stringTableKey;

		public Locale DefaultLocale => _defaultLocale;
		public string StringTableKey => _stringTableKey;
	}
}