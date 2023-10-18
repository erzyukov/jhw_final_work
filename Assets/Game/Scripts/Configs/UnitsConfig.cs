namespace Game.Configs
{
	using Units;
	using System;
	using UnityEngine;

	[CreateAssetMenu(fileName = "Units", menuName = "Configs/Units", order = (int)Config.Units)]
	public class UnitsConfig : ScriptableObject
	{
		[SerializeField] private UnitPrefab[] _prefabs;

		public UnitPrefab[] Prefabs => _prefabs;

		[Serializable]
		public struct UnitPrefab
		{
			public Unit.Type Type;
			public UnitView Prefab;
		}
    }
}