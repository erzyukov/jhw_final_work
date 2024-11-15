namespace Game.Configs
{
	using Game.Units;
	using System;
	using System.Linq;
	using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif

	[CreateAssetMenu(fileName = "Wave", menuName = "Configs/Wave", order = (int)Config.Wave)]
	public class WaveConfig : ScriptableObject
	{
		[SerializeField] private int _summonCurrencyAmount;
		[SerializeField] private WaveUnit[] _units;

		public int SummonCurrencyAmount => _summonCurrencyAmount;

		public WaveUnit[] Units { get { return _units; } set { _units = value; } }

		[Serializable]
		public struct WaveUnit
		{
			public Species Species;
			public int Power;
			public Vector2Int Position;
			public int GradeIndex;
		}
	}

#if UNITY_EDITOR
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

				int totalReward = waveConfig.Units.Sum(unit => 
					Mathf.CeilToInt(
						unitsConfig.Units[unit.Species].SoftReward + 
						unitsConfig.Units[unit.Species].SoftRewardPowerMultiplier * unit.Power
					)
				);
				int totalExperience = waveConfig.Units.Sum(unit =>
					Mathf.CeilToInt(
						unitsConfig.Units[unit.Species].Experience +
						unitsConfig.Units[unit.Species].ExperiencePowerMultiplier * unit.Power
					)
				);

				int totalHealth = waveConfig.Units.Sum(unit =>
					Mathf.CeilToInt(
						unitsConfig.Units[unit.Species].Health +
						unitsConfig.Units[unit.Species].HealthPowerMultiplier * unit.Power
					)
				);

				float totalAvrDamage = waveConfig.Units.Sum(unit =>
					Mathf.Round(
						(unitsConfig.Units[unit.Species].Damage +
						unitsConfig.Units[unit.Species].DamagePowerMultiplier * unit.Power) /
						unitsConfig.Units[unit.Species].AttackDelay *
						100
					) / 100
				);

				EditorGUILayout.LabelField("Total reward: ", totalReward.ToString());
				EditorGUILayout.LabelField("Total experience: ", totalExperience.ToString());

				EditorGUILayout.LabelField("Total health: ", totalHealth.ToString());
				EditorGUILayout.LabelField("Total avr damage: ", totalAvrDamage.ToString());
			}
			else
			{
				EditorGUILayout.LabelField($"Can't found UnitsConfig with name {unitsConfigAssetName}");
			}
		}
	}
#endif
}