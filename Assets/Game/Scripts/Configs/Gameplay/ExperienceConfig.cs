namespace Game.Configs
{
	using System;
	using System.Linq;
	using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif

	[CreateAssetMenu(fileName = "Experience", menuName = "Configs/Experience", order = (int)Config.Experience)]
	public class ExperienceConfig : ScriptableObject
	{
		[SerializeField] private HeroLevelData[] _heroLevels;

		public HeroLevelData[] HeroLevels => _heroLevels;

		[Serializable]
		public struct HeroLevelData
		{
			public int ExperienceToLevel;
			public int SoftCurrencyReward;
			public int HardCurrencyReward;
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(ExperienceConfig)), CanEditMultipleObjects]
	public class ExperienceConfigEditor : ConfigEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Game Balance", EditorStyles.boldLabel);

			ExperienceConfig experienceConfig = (ExperienceConfig)target;

			int totalExperience = experienceConfig.HeroLevels.Sum(level => level.ExperienceToLevel);
			int totalSoftCurrency = experienceConfig.HeroLevels.Sum(level => level.SoftCurrencyReward);
			int totalHardCurrency = experienceConfig.HeroLevels.Sum(level => level.HardCurrencyReward);

			EditorGUILayout.LabelField("Total experience: ", totalExperience.ToString());
			EditorGUILayout.LabelField("Total soft: ", totalSoftCurrency.ToString());
			EditorGUILayout.LabelField("Total hard: ", totalHardCurrency.ToString());
		}
	}
#endif
}