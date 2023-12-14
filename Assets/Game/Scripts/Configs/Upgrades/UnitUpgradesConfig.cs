namespace Game.Configs
{
	using System;
	using UnityEngine;

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
}