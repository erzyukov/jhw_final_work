namespace Game.Core
{
	using Game.Utilities;
	using UnityEngine;
	using Zenject;
	using UniRx;
	using Game.Units;
	using Game.Configs;
	using Game.Profiles;
	using Game.Managers;
	using System;

	public interface IGameUpgrades
	{
		ReactiveCommand<Species> Upgraded { get; }
		int GetUnitPower( Species species );
		int GetUnitLevel( Species species );
		int GetUpgradePrice( Species species );
		bool TryBuyUpgrade( Species species );
		bool CanUpgradeByLevel( Species species );
	}

	public class GameUpgrades : ControllerBase, IGameUpgrades, IInitializable
	{
		[Inject] private UpgradesConfig _upgradesConfig;
		[Inject] private GameProfile _gameProfile;
		[Inject] private IGameProfileManager _gameProfileManager;
		[Inject] private IGameCurrency _gameCurrency;
		[Inject] private IAdsManager _adsManager;

		public void Initialize()
		{
			_adsManager.OnCompleted[ERewardedType.UnitUpgrade]
				.Subscribe( OnAdRewardedUpgrade )
				.AddTo( this );
		}

		private void OnAdRewardedUpgrade( Rewarded rewarded )
		{
			Upgrade( rewarded.UpgradeUnit.Value );
		}

		#region IGameUpgrades

		public ReactiveCommand<Species> Upgraded { get; } = new ReactiveCommand<Species>();

		public int GetUnitPower( Species species ) =>
			( _gameProfile.Units.Upgrades[species].Value - 1 ) * _upgradesConfig.UpgradePowerBonus;

		public int GetUnitLevel( Species species ) =>
			_gameProfile.Units.Upgrades[species].Value;

		public int GetUpgradePrice( Species species )
		{
			UnitUpgradesConfig upgrade = _upgradesConfig.UnitsUpgrades[species];
			int level = Mathf.Clamp(_gameProfile.Units.Upgrades[species].Value, 0, upgrade.Price.Length);

			return upgrade.Price[level];
		}

		public bool TryBuyUpgrade( Species species )
		{
			int price = GetUpgradePrice(species);

			if (_gameCurrency.TrySpendSoftCurrency(
					price,
					SoftTransaction.Upgrade,
					$"{(int)species}_lvl_{_gameProfile.Units.Upgrades[species].Value + 1}"
					) == false
				)
				return false;

			Upgrade( species );

			return true;
		}

		public bool CanUpgradeByLevel( Species species ) =>
			_gameProfile.Units.Upgrades[species].Value < _gameProfile.HeroLevel.Value;

		#endregion

		private void Upgrade( Species species )
		{
			_gameProfile.Units.Upgrades[species].Value++;
			Upgraded.Execute( species );
			Save();
		}

		private void Save() => _gameProfileManager.Save();
	}
}