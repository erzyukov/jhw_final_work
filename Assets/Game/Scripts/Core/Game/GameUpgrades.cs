using static Game.Configs.UnitUpgradesConfig;

namespace Game.Core
{
	using Game.Utilities;
	using UnityEngine;
	using Zenject;
	using UniRx;
	using Game.Units;
	using Game.Configs;
	using Game.Profiles;
	using UpgradeData = Game.Configs.UnitUpgradesConfig.UpgradeData;

	public interface IGameUpgrades
	{
		float GetUnitHealth(Species species, int grade = 0);
		float GetUnitDamage(Species species, int grade = 0);
		float GetUnitHealthUpgradeDelta(Species species, int grade = 0);
		float GetUnitDamageUpgradeDelta(Species species, int grade = 0);
	}

	public class GameUpgrades : ControllerBase, IGameUpgrades, IInitializable
	{
		[Inject] UpgradesConfig _upgradesConfig;
		[Inject] UnitsConfig _unitsConfig;
		[Inject] GameProfile _gameProfile;
		[Inject] IGameProfileManager _gameProfileManager;

		public void Initialize()
		{
		}

		#region IGameUpgrades

		public float GetUnitHealth(Species species, int grade = 0)
		{
			float defaultHealth = _unitsConfig.Units[species].Grades[grade].Health;

			return defaultHealth + Mathf.Ceil(defaultHealth * GetUpgradeData(species).Health);
		}

		public float GetUnitDamage(Species species, int grade = 0)
		{
			float defaultDamage = _unitsConfig.Units[species].Grades[grade].Damage;

			return defaultDamage + Mathf.Ceil(defaultDamage * GetUpgradeData(species).Damage);
		}

		public float GetUnitHealthUpgradeDelta(Species species, int grade = 0)
		{
			float defaultHealth = _unitsConfig.Units[species].Grades[0].Health;
			int level = _gameProfile.Units.Upgrades[species];

			return Mathf.Ceil(defaultHealth * GetUpgradeData(species, level + 1).Health);
		}

		public float GetUnitDamageUpgradeDelta(Species species, int grade = 0)
		{
			float defaultDamage = _unitsConfig.Units[species].Grades[0].Damage;
			int level = _gameProfile.Units.Upgrades[species];

			return Mathf.Ceil(defaultDamage * GetUpgradeData(species, level + 1).Damage);
		}

		#endregion

		private UpgradeData GetUpgradeData(Species species)
		{
			int level = _gameProfile.Units.Upgrades[species];

			return GetUpgradeData(species, level);
		}

		private UpgradeData GetUpgradeData(Species species, int level)
		{
			UnitUpgradesConfig upgrade = _upgradesConfig.UnitsUpgrades[species];

			return upgrade.Upgrades[level];
		}
	}
}
