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
	using System.Collections.Generic;
	using System.Linq;

	public interface IGameUpgrades
	{
		ReactiveCommand<Species> Upgraded { get; }
		int GetUnitLevel(Species species);
		float GetUnitHealth(Species species);
		float GetUnitDamage(Species species);
		float GetUnitHealthUpgradeDelta(Species species);
		float GetUnitDamageUpgradeDelta(Species species);
		int GetUpgradePrice(Species species);
		bool TryBuyUpgrade(Species species);
	}

	public class GameUpgrades : ControllerBase, IGameUpgrades, IInitializable
	{
		[Inject] UpgradesConfig _upgradesConfig;
		[Inject] UnitsConfig _unitsConfig;
		[Inject] GameProfile _gameProfile;
		[Inject] IGameProfileManager _gameProfileManager;
		[Inject] IGameCurrency _gameCurrency;

		private Dictionary<Species, float> _currentHealth = new Dictionary<Species, float>();
		private Dictionary<Species, float> _currentDamage = new Dictionary<Species, float>();

		public void Initialize()
		{
            foreach (var upgrade in _gameProfile.Units.Upgrades)
            {
				upgrade.Value
					.Subscribe(level => OnUnitUpgradedHandler(upgrade.Key, level))
					.AddTo(this);
			}

			SetupEnemyUnits();
		}

		#region IGameUpgrades

		public ReactiveCommand<Species> Upgraded { get; } = new ReactiveCommand<Species>();

		public int GetUnitLevel(Species species) =>
			_gameProfile.Units.Upgrades[species].Value;

		public float GetUnitHealth(Species species) =>
			_currentHealth[species];

		public float GetUnitDamage(Species species) =>
			_currentDamage[species];

		public float GetUnitHealthUpgradeDelta(Species species)
		{
			float defaultHealth = _unitsConfig.Units[species].Health;

			return Mathf.Ceil(defaultHealth * GetNextUpgradeData(species).Health);
		}

		public float GetUnitDamageUpgradeDelta(Species species)
		{
			float defaultDamage = _unitsConfig.Units[species].Damage;

			return Mathf.Ceil(defaultDamage * GetNextUpgradeData(species).Damage);
		}

		public int GetUpgradePrice(Species species) =>
			GetNextUpgradeData(species).Price;

		public bool TryBuyUpgrade(Species species)
		{
			int price = GetUpgradePrice(species);
			
			if (_gameCurrency.TrySpendSoftCurrency(price) == false)
				return false;

			_gameProfile.Units.Upgrades[species].Value++;
			Upgraded.Execute(species);
			Save();
			
			return true;
		}

		#endregion

		private void OnUnitUpgradedHandler(Species species, int level)
		{
			SetupCurrenUpgradeValues(species, level);
		}

		private void SetupEnemyUnits()
		{
			List<Species> enemyUnits = _unitsConfig.Units
				.Where(upgrade => _unitsConfig.HeroUnits.Contains(upgrade.Key) == false)
				.Select(upgrade => upgrade.Key)
				.ToList();
			
			foreach (var species in enemyUnits)
			{
				_currentHealth[species] = _unitsConfig.Units[species].Health;
				_currentDamage[species] = _unitsConfig.Units[species].Damage;
			}
		}

		private void SetupCurrenUpgradeValues(Species species, int level)
		{
			_currentHealth[species] = _unitsConfig.Units[species].Health;
			_currentDamage[species] = _unitsConfig.Units[species].Damage;

			for (int i = 1; i <= level; i++)
			{
				UpgradeData data = GetUpgradeData(species, i);

				_currentHealth[species] += Mathf.Ceil(data.Health * _unitsConfig.Units[species].Health);
				_currentDamage[species] += Mathf.Ceil(data.Damage * _unitsConfig.Units[species].Damage);
			}
		}

		private UpgradeData GetUpgradeData(Species species)
		{
			int level = _gameProfile.Units.Upgrades[species].Value;

			return GetUpgradeData(species, level);
		}

		private UpgradeData GetNextUpgradeData(Species species)
		{
			int level = _gameProfile.Units.Upgrades[species].Value;

			return GetUpgradeData(species, level + 1);
		}

		private UpgradeData GetUpgradeData(Species species, int levelNumber)
		{
			UnitUpgradesConfig upgrade = _upgradesConfig.UnitsUpgrades[species];

			if (levelNumber < upgrade.Upgrades.Length)
				return upgrade.Upgrades[levelNumber - 1];
			else
				return upgrade.Upgrades[0];
		}

		private void Save() => _gameProfileManager.Save();
	}
}
