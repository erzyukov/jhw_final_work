namespace Game.Configs
{
	using System;
	using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
	using System.Linq;
#endif

	[CreateAssetMenu(fileName = "UnitUpgrades", menuName = "Configs/UnitUpgrades", order = (int)Config.UnitUpgrades)]
	public class UnitUpgradesConfig : ScriptableObject
	{
		[SerializeField] private UpgradeData[] _upgrades;

		public UpgradeData[] Upgrades => _upgrades;

		[Serializable]
		public struct UpgradeData
		{
			public int Price;
			public float Health;
			public float Damage;
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(UnitUpgradesConfig)), CanEditMultipleObjects]
	public class UnitUpgradesConfigEditor : ConfigEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Game Balance", EditorStyles.boldLabel);

			UnitUpgradesConfig config = (UnitUpgradesConfig)target;

			int cost = config.Upgrades.Sum(upgrade => upgrade.Price);

			EditorGUILayout.LabelField("Total upgrades cost: ", cost.ToString());
		}
	}
#endif
}