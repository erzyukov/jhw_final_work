﻿namespace Game.Ui
{
	using Game.Configs;
	using Game.Core;
	using Game.Profiles;
	using Game.Units;
	using Game.Utilities;
	using UniRx;
	using UnityEngine;
	using Zenject;
	using static Game.Configs.UnitsConfig;

	public class UiUpgrades : ControllerBase, IInitializable
	{
		[Inject] private IUiUpgradesScreen _screen;
		[Inject] private UnitsConfig _unitsConfig;
		[Inject] private UpgradesConfig _upgradesConfig;
		[Inject] private ILocalizator _localizator;
		[Inject] private GameProfile _gameProfile;
		[Inject] private IGameUpgrades _gameUpgrades;
		[Inject] private IUiMessage _uiMessage;


		private const string LevelTitleKey = "unitLevel";
		private const string LevelShortTitleKey = "lvl";
		private const int DummyElementsCount = 3;

		private Species _firstElement;

		public void Initialize()
		{
			_screen.UnitsContainer.DestroyChildren();
			CreateUnitList();
			FillUnitInfo(_firstElement);
		}

		private void FillUnitInfo(Species species)
		{
			if (_unitsConfig.Units.TryGetValue(species, out var unit) == false)
				return;

			int unitLevel = _gameProfile.Units.Upgrades[species].Value;

			int power = _gameUpgrades.GetUnitPower(species);
			int health = Mathf.CeilToInt(unit.Health + unit.HealthPowerMultiplier * power);
			int healthUpgradeDelta = Mathf.CeilToInt(unit.HealthPowerMultiplier * _upgradesConfig.UpgradePowerBonus);
			int damage = Mathf.CeilToInt(unit.Damage + unit.DamagePowerMultiplier * power);
			int damageUpgradeDelta = Mathf.CeilToInt(unit.DamagePowerMultiplier * _upgradesConfig.UpgradePowerBonus);

			_screen.SetIcon(unit.Icon);
			_screen.SetName(_localizator.GetString(unit.TitleKey));
			_screen.SetLevel($"{_localizator.GetString(LevelTitleKey)} {unitLevel}");
			_screen.SetHealthValue(
				health.ToString(),
				healthUpgradeDelta.ToString()
			);
			_screen.SetDamageValue(
				damage.ToString(),
				damageUpgradeDelta.ToString()
			);
			_screen.SetSpeedValue($"{unit.AttackDelay}s");
			_screen.SetRangeValue($"{unit.AttackRange}");
		}

		private void CreateUnitList()
		{
			for (int i = 0; i < DummyElementsCount; i++)
				GameObject.Instantiate(_screen.UnitUnavailableDummyPrefab, _screen.UnitsContainer);

            for (int i = _unitsConfig.HeroUnits.Count - 1; i >= 0; i--)
            {
				Species species = _unitsConfig.HeroUnits[i];

				if (_unitsConfig.Units.TryGetValue(species, out var unit) == false)
					continue;

				if (_screen.UnitElements.Count == _unitsConfig.HeroUnits.Count - 1)
					_firstElement = species;

				int unitLevel = _gameProfile.Units.Upgrades[species].Value;

				UiUnitUpgradeElement element = GameObject.Instantiate(_screen.UnitElementPrefab, _screen.UnitsContainer);
				element.SetIcon(unit.Icon);
				element.SetLevel($"{_localizator.GetString(LevelShortTitleKey)} {unitLevel}");
				element.SetPrice(_gameUpgrades.GetUpgradePrice(species).ToString());
				_screen.UnitElements.Add(species, element);

				element.SelectButtonClicked
					.Subscribe(_ => FillUnitInfo(species))
					.AddTo(this);

				element.UpgradeButtonClicked
					.Subscribe(_ => OnUpgradeButtonClickedHandler(species))
					.AddTo(this);
			}
		}

		private void UpdateUnitElement(Species species)
		{
			UiUnitUpgradeElement element = _screen.UnitElements[species];
			int unitLevel = _gameProfile.Units.Upgrades[species].Value;
			element.SetLevel($"{_localizator.GetString(LevelShortTitleKey)} {unitLevel}");
			element.SetPrice(_gameUpgrades.GetUpgradePrice(species).ToString());
		}

		private void OnUpgradeButtonClickedHandler(Species species)
		{
			if (_gameUpgrades.TryBuyUpgrade(species) == false)
			{
				_uiMessage.ShowMessage(UiMessage.NotEnoughSoftCurrency);
			}
			else
			{
				foreach (Species unitSpecies in _unitsConfig.HeroUnits)
					UpdateUnitElement(unitSpecies);
			}

			FillUnitInfo(species);
		}
	}
}
