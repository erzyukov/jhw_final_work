namespace Game.Configs
{
	using Game.Units;
	using System;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	[CreateAssetMenu(fileName = "Wave", menuName = "Configs/Wave", order = (int)Config.Wave)]
	public class WaveConfig : ScriptableObject
	{
		[SerializeField] private int _summonCurrencyAmount;
		[SerializeField] private WaveUnit[] _units;

		public int SummonCurrencyAmount => _summonCurrencyAmount;

		public WaveUnit[] Units => _units;

		[Serializable]
		public struct WaveUnit
		{
			public Species Species;
			public int GradeIndex;
			public Vector2Int Position;
		}
	}

	[CustomEditor(typeof(WaveConfig)), CanEditMultipleObjects]
	public class WaveConfigEditor : ConfigEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Game Balance", EditorStyles.boldLabel);

			UnitsConfig unitsConfig = GetUnitsConfig(out string unitsConfigAssetName);

			if (unitsConfig != null)
			{
				WaveConfig waveConfig = (WaveConfig)target;

				int totalReward = waveConfig.Units.Sum(unit => unitsConfig.Units[unit.Species].Grades[unit.GradeIndex].SoftCurrencyReward);
				int totalExperience = waveConfig.Units.Sum(unit => unitsConfig.Units[unit.Species].Grades[unit.GradeIndex].ExperienceReward);

				EditorGUILayout.LabelField("Total reward: ", totalReward.ToString());
				EditorGUILayout.LabelField("Total experience: ", totalExperience.ToString());
			}
			else
			{
				EditorGUILayout.LabelField($"Can't found UnitsConfig with name {unitsConfigAssetName}");
			}
		}
	}
}