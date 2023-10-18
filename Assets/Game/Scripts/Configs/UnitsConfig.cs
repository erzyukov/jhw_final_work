namespace Game.Configs
{
	using Units;
	using System;
	using UnityEngine;
	using System.Linq;

	[CreateAssetMenu(fileName = "Units", menuName = "Configs/Units", order = (int)Config.Units)]
	public class UnitsConfig : ScriptableObject
	{
		[SerializeField] private UnitPrefab[] _prefabs;

		public UnitPrefab[] Prefabs => _prefabs;

		public UnitView GetPrefab(Unit.Type type) => 
			_prefabs.Where(element => element.Type == type).First().Prefab;

		[Serializable]
		public struct UnitPrefab
		{
			public Unit.Type Type;
			public UnitView Prefab;
		}
    }
}