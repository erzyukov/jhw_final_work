namespace Game.Configs
{
	using Game.Units;
	using System;
	using System.Collections.Generic;
	using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
	using System.Linq;
#endif

	[CreateAssetMenu(fileName = "Upgrades", menuName = "Configs/Upgrades", order = (int)Config.Upgrades)]
	public class UpgradesConfig : ScriptableObject
	{
		[SerializeField] private int _upgradePowerBonus;
		[SerializeField] private UnitUpgrades[] _unitsUpgrades;

		private Dictionary<Species, UnitUpgradesConfig> _unitData;

		public int UpgradePowerBonus => _upgradePowerBonus;

		public Dictionary<Species, UnitUpgradesConfig> UnitsUpgrades => _unitData;

		public void Initialize()
		{
			_unitData = new Dictionary<Species, UnitUpgradesConfig>();

			foreach (var unit in _unitsUpgrades)
				_unitData.Add(unit.Species, unit.Upgrades);
		}

		[Serializable]
		public struct UnitUpgrades
		{
			public Species Species;
			public UnitUpgradesConfig Upgrades;
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(UpgradesConfig)), CanEditMultipleObjects]
	public class UpgradesConfigEditor : ConfigEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Game Balance", EditorStyles.boldLabel);

			UpgradesConfig config = (UpgradesConfig)target;
			config.Initialize();
			int cost = config.UnitsUpgrades.Sum(u => u.Value.Upgrades.Sum(upgrade => upgrade.Price));

			EditorGUILayout.LabelField("Total upgrades cost: ", cost.ToString());
		}
	}
#endif
}