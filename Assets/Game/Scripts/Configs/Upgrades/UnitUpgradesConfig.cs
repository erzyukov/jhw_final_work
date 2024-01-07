namespace Game.Configs
{
	using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
	using System.Linq;
#endif

	[CreateAssetMenu(fileName = "UnitUpgrades", menuName = "Configs/UnitUpgrades", order = (int)Config.UnitUpgrades)]
	public class UnitUpgradesConfig : ScriptableObject
	{
		[SerializeField] private int[] _price;

		public int[] Price => _price;
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

			int cost = config.Price.Sum();

			EditorGUILayout.LabelField("Total upgrades cost: ", cost.ToString());
		}
	}
#endif
}