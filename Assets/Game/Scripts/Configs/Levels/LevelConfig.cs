namespace Game.Configs
{
	using System.Linq;
	using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif

	[CreateAssetMenu(fileName = "Level", menuName = "Configs/Level", order = (int)Config.Level)]
	public class LevelConfig : ScriptableObject
	{
		[SerializeField] private string _title;
		[SerializeField] private Region _region;
		[SerializeField] private int _softCurrencyReward;
		[SerializeField] private WaveConfig[] _waves;

		public string Title => _title;
		public Region Region => _region;
		public WaveConfig[] Waves => _waves;
		public int SoftCurrencyReward => _softCurrencyReward;
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(LevelConfig)), CanEditMultipleObjects]
	public class LevelConfigEditor : ConfigEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Game Balance", EditorStyles.boldLabel);

			UnitsConfig unitsConfig = GetUnitsConfig(out string unitsConfigAssetName);

			if (unitsConfig != null)
			{
				LevelConfig levelConfig = (LevelConfig)target;

				int totalReward = levelConfig.Waves.Sum(w => w.Units.Sum(unit => unitsConfig.Units[unit.Species].Grades[unit.GradeIndex].SoftCurrencyReward));
				int totalExperience = levelConfig.Waves.Sum(w => w.Units.Sum(unit => unitsConfig.Units[unit.Species].Grades[unit.GradeIndex].ExperienceReward));

				totalReward += levelConfig.SoftCurrencyReward;

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