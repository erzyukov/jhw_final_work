namespace Game.Ui
{
	using Game.Configs;
	using Game.Core;
	using Game.Units;
	using Game.Utilities;
	using System.Collections.Generic;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public class UiUpgrades : ControllerBase, IInitializable
	{
		[Inject] private IUiUpgradesScreen _screen;
		[Inject] private UnitsConfig _unitsConfig;
		[Inject] private ILocalizator _localizator;

		private const string LevelTitleKey = "unitLevel";
		private const string LevelShortTitleKey = "lvl";
		private const int DummyElementsCount = 3;

		// TODO: replace value from upgrade profile
		private const string UnitLevel = "4";

		private Species _firstElement;
		private List<UiUnitUpgradeElement> _unitElements = new List<UiUnitUpgradeElement>();

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

			_screen.SetIcon(unit.Icon);
			_screen.SetName(unit.Title);
			_screen.SetLevel($"{_localizator.GetString(LevelTitleKey)} {UnitLevel}");
			_screen.SetHealthValue($"{grade.Health}");
			_screen.SetDamageValue($"{grade.Damage}");
			_screen.SetSpeedValue($"{grade.AttackDelay}s");
			_screen.SetRangeValue($"{unit.AttackRange}");
		}

		private void CreateUnitList()
		{
			foreach (var species in _unitsConfig.HeroUnits)
			{
				if (_unitsConfig.Units.TryGetValue(species, out var unit) == false)
					continue;

				if (_unitElements.Count == 0)
					_firstElement = species;

				UiUnitUpgradeElement element = GameObject.Instantiate(_screen.UnitElementPrefab);
				element.SetIcon(unit.Icon);
				element.SetLevel($"{_localizator.GetString(LevelShortTitleKey)} {UnitLevel}");
				element.SetParent(_screen.UnitsContainer);
				_unitElements.Add(element);

				element.SelectButtonClicked
					.Subscribe(_ => FillUnitInfo(species))
					.AddTo(this);
			}

			for (int i = 0; i < DummyElementsCount; i++)
			{
				GameObject.Instantiate(_screen.UnitUnavailableDummyPrefab).transform
					.SetParent(_screen.UnitsContainer);
			}
		}
	}
}
