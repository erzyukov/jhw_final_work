namespace Game.Ui
{
	using Game.Configs;
	using Game.Core;
	using Game.Profiles;
	using Game.Units;
	using Game.Utilities;
	using System.Linq;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public class UiUpgrades : ControllerBase, IInitializable
	{
		[Inject] private IUiUpgradesScreen _screen;
		[Inject] private UnitsConfig _unitsConfig;
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

			UnitGrade grade = unit.Grades[0];
			int unitLevel = _gameProfile.Units.Upgrades[species].Value;

			_screen.SetIcon(unit.Icon);
			_screen.SetName(unit.Title);
			_screen.SetLevel($"{_localizator.GetString(LevelTitleKey)} {unitLevel}");
			_screen.SetHealthValue(
				_gameUpgrades.GetUnitHealth(species).ToString(),
				_gameUpgrades.GetUnitHealthUpgradeDelta(species).ToString()
			);
			_screen.SetDamageValue(
				_gameUpgrades.GetUnitDamage(species).ToString(),
				_gameUpgrades.GetUnitDamageUpgradeDelta(species).ToString()
			);
			_screen.SetSpeedValue($"{grade.AttackDelay}s");
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

				if (_screen.UnitElements.Count == 0)
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
