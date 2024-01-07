namespace Game.Core
{
	using Game.Utilities;
	using UnityEngine;
	using Zenject;
	using UniRx;
	using Game.Units;
	using Game.Configs;
	using Game.Profiles;

	public interface IGameUpgrades
	{
		ReactiveCommand<Species> Upgraded { get; }
		int GetUnitPower(Species species);
		int GetUnitLevel(Species species);
		int GetUpgradePrice(Species species);
		bool TryBuyUpgrade(Species species);
	}

	public class GameUpgrades : ControllerBase, IGameUpgrades
	{
		[Inject] UpgradesConfig _upgradesConfig;
		[Inject] GameProfile _gameProfile;
		[Inject] IGameProfileManager _gameProfileManager;
		[Inject] IGameCurrency _gameCurrency;

		#region IGameUpgrades

		public ReactiveCommand<Species> Upgraded { get; } = new ReactiveCommand<Species>();

		public int GetUnitPower(Species species) => 
			(_gameProfile.Units.Upgrades[species].Value - 1) * _upgradesConfig.UpgradePowerBonus;

		public int GetUnitLevel(Species species) =>
			_gameProfile.Units.Upgrades[species].Value;

		public int GetUpgradePrice(Species species)
		{
			UnitUpgradesConfig upgrade = _upgradesConfig.UnitsUpgrades[species];
			int level = Mathf.Clamp(_gameProfile.Units.Upgrades[species].Value, 0, upgrade.Price.Length);

			return upgrade.Price[level - 1];
		}

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

		private void Save() => _gameProfileManager.Save();
	}
}