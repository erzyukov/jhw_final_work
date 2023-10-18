namespace Game.Configs
{
	using Units;
	using System;
	using UnityEngine;

	[CreateAssetMenu(fileName = "CombatUnits", menuName = "Configs/CombatUnits", order = (int)Config.CombatUnits)]
	public class CombatUnitsConfig : ScriptableObject
	{
		[SerializeField] private CombatUnitPrefab[] _prefabs;

		public CombatUnitPrefab[] Prefabs => _prefabs;

		[Serializable]
		public struct CombatUnitPrefab
		{
			public CombatUnit.Type Type;
			public CombatUnitView Prefab;
		}
    }
}