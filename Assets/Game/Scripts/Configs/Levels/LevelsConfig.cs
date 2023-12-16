namespace Game.Configs
{
	using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
	using System.Linq;
#endif

	[CreateAssetMenu(fileName = "Levels", menuName = "Configs/Levels", order = (int)Config.Levels)]
	public class LevelsConfig : ScriptableObject
	{
		[SerializeField] private LevelConfig[] _levels;

		public LevelConfig[] Levels => _levels;
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(LevelsConfig)), CanEditMultipleObjects]
	public class LevelsConfigEditor : ConfigEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Game Balance", EditorStyles.boldLabel);

			int totalReward = 0;
			int totalExperience = 0;
			LevelsConfig config = (LevelsConfig)target;
			UnitsConfig unitsConfig = GetUnitsConfig(out string unitsConfigAssetName);

			if (unitsConfig != null)
			{
				foreach (var levelConfig in config.Levels)
				{
					totalReward += levelConfig.Waves.Sum(w => w.Units.Sum(unit => unitsConfig.Units[unit.Species].Grades[unit.GradeIndex].SoftCurrencyReward));
					totalExperience += levelConfig.Waves.Sum(w => w.Units.Sum(unit => unitsConfig.Units[unit.Species].Grades[unit.GradeIndex].ExperienceReward));
				}

				EditorGUILayout.LabelField("Total reward: ", totalReward.ToString());
				EditorGUILayout.LabelField("Total experience: ", totalExperience.ToString());
			}
			else
			{
				EditorGUILayout.LabelField($"Can't found UnitsConfig with name {unitsConfigAssetName}");
			}
		}
	}
#endif
}