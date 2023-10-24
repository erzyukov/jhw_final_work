namespace Game.Configs
{
	using Units;
	using System;
	using UnityEngine;
	using System.Collections.Generic;

	[CreateAssetMenu(fileName = "Units", menuName = "Configs/Units", order = (int)Config.Units)]
	public class UnitsConfig : ScriptableObject
	{
		[SerializeField] private UnitData[] _units;

		private Dictionary<Unit.Kind, UnitConfig> _unitData;

		public Dictionary<Unit.Kind, UnitConfig> Units => _unitData;

		public void Initialize()
		{
			_unitData = new Dictionary<Unit.Kind, UnitConfig>();
            foreach (var unit in _units)
				_unitData.Add(unit.Type, unit.Config);
		}
		
		[Serializable]
		public struct UnitData
		{
			public Unit.Kind Type;
			public UnitConfig Config;
		}
    }
}