namespace Game.Configs
{
	using Game.Units;
	using System;
	using UnityEngine;
	using System.Collections.Generic;

	[CreateAssetMenu(fileName = "Units", menuName = "Configs/Units", order = (int)Config.Units)]
	public class UnitsConfig : ScriptableObject
	{
		[SerializeField] private GameObject _unitPrefab;
		[SerializeField] private float _speed;
		[SerializeField] private UnitData[] _units;

		private Dictionary<Species, UnitConfig> _unitData;

		public GameObject UnitPrefab => _unitPrefab;
		public float Speed => _speed;
		public Dictionary<Species, UnitConfig> Units => _unitData;
		
		public void Initialize()
		{
			_unitData = new Dictionary<Species, UnitConfig>();
            foreach (var unit in _units)
				_unitData.Add(unit.Species, unit.Config);
		}
		
		[Serializable]
		public struct UnitData
		{
			public Species Species;
			public UnitConfig Config;
		}
    }
}