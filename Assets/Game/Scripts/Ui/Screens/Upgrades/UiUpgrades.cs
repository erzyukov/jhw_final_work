namespace Game.Ui
{
	using Game.Configs;
	using Game.Core;
	using Game.Managers;
	using Game.Profiles;
	using Game.Units;
	using Game.Utilities;
	using Sirenix.Utilities;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public class UiUpgrades : ControllerBase, IInitializable
	{
		[Inject] private IUiUpgradesScreen _screen;
		[Inject] private UnitsConfig _unitsConfig;
		[Inject] private UpgradesConfig _upgradesConfig;
		[Inject] private ILocalizator _localizator;
		[Inject] private GameProfile _gameProfile;
		[Inject] private IGameUpgrades _gameUpgrades;
		[Inject] private IUiMessage _uiMessage;
		[Inject] private IGameAudio _gameAudio;
		[Inject] private IResourceEvents _resourceEvents;
		[Inject] private IAdsManager _adsManager;

		private const string LevelTitleKey = "unitLevel";
		private const string LevelShortTitleKey = "lvl";
		private const int DummyElementsCount = 3;

		private Species _firstElement;

		public void Initialize()
		{
			_screen.UnitsContainer.DestroyChildren();
			CreateUnitList();
			FillUnitInfo( _firstElement );
			UpdateUnitElement( _firstElement, true );

			Observable.Merge(
				_gameUpgrades.Upgraded.AsUnitObservable(),
				_gameProfile.SoftCurrency.AsUnitObservable(),
				_gameProfile.HeroLevel.AsUnitObservable(),
				_adsManager.IsRewardedAvailable.AsUnitObservable()
			)
				.Subscribe( _ => OnSoftCurrencyChanged() )
				.AddTo( this );

			_gameUpgrades.Upgraded
				.Subscribe( OnUnitUpgraded )
				.AddTo( this );
		}

		private void OnSoftCurrencyChanged()
		{
			int heroLevel = _gameProfile.HeroLevel.Value;
			int ccy = _gameProfile.SoftCurrency.Value;
			bool isRewardAvailable = _adsManager.IsRewardedAvailable.Value;

			_screen.UnitElements.ForEach( element =>
			{
				int unitLevel = _gameUpgrades.GetUnitLevel( element.Key );
				int upgradePrice = _gameUpgrades.GetUpgradePrice( element.Key );
				bool isAdAcitve = (upgradePrice > ccy) && isRewardAvailable && unitLevel < heroLevel;

				element.Value.UpgradeButton.SetAdActive( isAdAcitve );
			} );

		}

		private void FillUnitInfo( Species species )
		{
			if (_unitsConfig.Units.TryGetValue( species, out var unit ) == false)
				return;

			int unitLevel = _gameProfile.Units.Upgrades[species].Value;

			int power = _gameUpgrades.GetUnitPower(species);
			int health = Mathf.CeilToInt(unit.Health + unit.HealthPowerMultiplier * power);
			int healthUpgradeDelta = Mathf.CeilToInt(unit.HealthPowerMultiplier * _upgradesConfig.UpgradePowerBonus);
			int damage = Mathf.CeilToInt(unit.Damage + unit.DamagePowerMultiplier * power);
			int damageUpgradeDelta = Mathf.CeilToInt(unit.DamagePowerMultiplier * _upgradesConfig.UpgradePowerBonus);

			_screen.SetIcon( unit.FullLength );
			_screen.SetName( _localizator.GetString( unit.TitleKey ) );
			_screen.SetLevel( $"{_localizator.GetString( LevelTitleKey )} {unitLevel}" );
			_screen.SetHealthValue(
				health.ToString(),
				healthUpgradeDelta.ToString()
			);
			_screen.SetDamageValue(
				damage.ToString(),
				damageUpgradeDelta.ToString()
			);
			_screen.SetSpeedValue( $"{unit.AttackDelay}s" );
			_screen.SetRangeValue( $"{unit.AttackRange}" );
		}

		private void CreateUnitList()
		{
			for (int i = 0; i < DummyElementsCount; i++)
				GameObject.Instantiate( _screen.UnitUnavailableDummyPrefab, _screen.UnitsContainer );

			for (int i = _unitsConfig.HeroUnits.Count - 1; i >= 0; i--)
			{
				Species species = _unitsConfig.HeroUnits[i];

				if (_unitsConfig.Units.TryGetValue( species, out var unit ) == false)
					continue;

				if (_screen.UnitElements.Count == _unitsConfig.HeroUnits.Count - 1)
					_firstElement = species;

				int unitLevel = _gameProfile.Units.Upgrades[species].Value;

				UiUnitUpgradeElement element = GameObject.Instantiate( _screen.UnitElementPrefab, _screen.UnitsContainer );
				element.SetIcon( unit.Icon );
				element.SetLevel( $"{_localizator.GetString( LevelShortTitleKey )} {unitLevel}" );
				element.SetTitle( _localizator.GetString( unit.TitleKey ) );
				element.SetPrice( _gameUpgrades.GetUpgradePrice( species ).ToString() );
				_screen.UnitElements.Add( species, element );

				element.SelectButtonClicked
					.Subscribe( _ => OnSelectButtonClicked( species ) )
					.AddTo( this );

				element.UpgradeButton.Clicked
					.Subscribe( _ => OnUpgradeButtonClickedHandler( species ) )
					.AddTo( this );

				element.UpgradeButton.AdClicked
					.Subscribe( _ => _adsManager.ShowRewardedVideo( ERewardedType.UnitUpgrade, new Rewarded( species ) ) )
					.AddTo( this );
			}
		}

		private void UpdateUnitElement( Species species, bool isSelected = false )
		{
			UiUnitUpgradeElement element = _screen.UnitElements[species];
			int unitLevel = _gameProfile.Units.Upgrades[species].Value;
			element.SetLevel( $"{_localizator.GetString( LevelShortTitleKey )} {unitLevel}" );
			element.SetPrice( _gameUpgrades.GetUpgradePrice( species ).ToString() );
			element.SetSelected( isSelected );
		}

		private void OnSelectButtonClicked( Species species )
		{
			_gameAudio.PlayUiClick();
			FillUnitInfo( species );

			foreach (Species unitSpecies in _unitsConfig.HeroUnits)
				_screen.UnitElements[unitSpecies].SetSelected( unitSpecies == species );
		}

		private void OnUpgradeButtonClickedHandler( Species species )
		{
			if (_gameUpgrades.CanUpgradeByLevel( species ) == false)
			{
				_uiMessage.ShowMessage( UiMessage.LowHeroLevel );
				_resourceEvents.LowHeroLevelAlert.Execute();
			}
			else if (_gameUpgrades.TryBuyUpgrade( species ) == false)
			{
				_uiMessage.ShowMessage( UiMessage.NotEnoughSoftCurrency );
			}
			else
			{
				UpdateUnitList( species );
			}

			_gameAudio.PlayUiClick();
			FillUnitInfo( species );
		}

		private void OnUnitUpgraded( Species species )
		{
			UpdateUnitList( species );
			FillUnitInfo( species );
		}

		private void UpdateUnitList( Species selected )
		{
			foreach (Species unitSpecies in _unitsConfig.HeroUnits)
				UpdateUnitElement( unitSpecies, unitSpecies == selected );
		}
	}
}
