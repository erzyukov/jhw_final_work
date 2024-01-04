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
		[SerializeField] private WaveConfig[] _waves;

		public string Title => _title;
		public Region Region => _region;
		public WaveConfig[] Waves => _waves;
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

				int totalReward = levelConfig.Waves.Sum(w => w.Units.Sum(unit =>
					Mathf.CeilToInt(
						unitsConfig.Units[unit.Species].SoftReward +
						unitsConfig.Units[unit.Species].SoftRewardPowerMultiplier * unit.Power
					))
				);
				int totalExperience = levelConfig.Waves.Sum(w => w.Units.Sum(unit =>
					Mathf.CeilToInt(
						unitsConfig.Units[unit.Species].Experience +
						unitsConfig.Units[unit.Species].ExperiencePowerMultiplier * unit.Power
					))
				);

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