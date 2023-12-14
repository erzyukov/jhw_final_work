namespace Game.Configs
{
	using Game.Units;
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "Upgrades", menuName = "Configs/Upgrades", order = (int)Config.Upgrades)]
	public class UpgradesConfig : ScriptableObject
	{
		[SerializeField] private UnitUpgrades[] _unitsUpgrades;

		private Dictionary<Species, UnitUpgradesConfig> _unitData;

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
}