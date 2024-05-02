namespace Game.Ui
{
	using Game.Configs;
	using Game.Core;
	using Game.Managers;
	using Game.Profiles;
	using Game.Units;
	using Game.Utilities;
	using Sirenix.Utilities;
	using System.Linq;
	using UniRx;
	using UnityEngine;
	using Zenject;
	using UnitViewArgs = UiUpgradeUnitViewFactory.Args;

	public class UiUpgrades : ControllerBase, IInitializable
	{
		[Inject] private IUiUpgradesScreen				_screen;
		[Inject] private UnitsConfig					_unitsConfig;
		[Inject] private UpgradesConfig					_upgradesConfig;
		[Inject] private UiCommonConfig					_uiCommonConfig;
		[Inject] private ILocalizator					_localizator;
		[Inject] private GameProfile					_gameProfile;
		[Inject] private IGameUpgrades					_gameUpgrades;
		[Inject] private IUiMessage						_uiMessage;
		[Inject] private IGameAudio						_gameAudio;
		[Inject] private IResourceEvents				_resourceEvents;
		[Inject] private IAdsManager					_adsManager;
		[Inject] private UiUpgradeUnitView.Factory		_upgradeUnitViewFactory;
		[Inject] private IUiUpgradeFlow					_flow;

		private const string	LevelTitleKey = "unitLevel";

		public void Initialize()
		{
			var first = _gameProfile.Units.Upgrades.First().Key;
			_screen.UnitsContainer.DestroyChildren();
			CreateUnitList();
			FillUnitInfo( first );

			_flow.SelectedUnit
				.Subscribe( OnUnitSelected )
				.AddTo( this );

			_flow.UpgradeClicked
				.Subscribe( OnUpgradeButtonClicked )
				.AddTo( this );

			/*
			Observable.Merge(
				_gameUpgrades.Upgraded.AsUnitObservable(),
				_gameProfile.SoftCurrency.AsUnitObservable(),
				_gameProfile.HeroLevel.AsUnitObservable(),
				_adsManager.IsRewardedAvailable.AsUnitObservable()
			)
				.Subscribe( _ => _flow.SoftCurrencyChanged.Execute() )
				.AddTo( this );
			*/

			_gameUpgrades.Upgraded
				.Subscribe( OnUnitUpgraded )
				.AddTo( this );
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

			bool isBlocked = unitLevel == 0;
			_screen.SetIcon( unit.FullLength );
			_screen.SetIconMaterial( isBlocked ? _uiCommonConfig.BlockedElementMaterial : null );
			_screen.SetName( unit.Name );
			_screen.SetLevel( unitLevel );
			_screen.SetLevelActive( !isBlocked );
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
			_gameProfile.Units.Upgrades.ForEach( kvp => CreateUnitItem( kvp.Key ) );
		}

		private void CreateUnitItem( Species species )
		{
			if (_unitsConfig.Units.TryGetValue( species, out var unit ) == false)
				return;

			UnitViewArgs args = new()
			{
				Species		= species,
				Config		= unit,
			};

			var item	= _upgradeUnitViewFactory.Create(args);

			item.SetParent( _screen.UnitsContainer );
		}

		private void OnUnitSelected( Species species )
		{
			_gameAudio.PlayUiClick();
			FillUnitInfo( species );
		}

		private void OnUpgradeButtonClicked( Species species )
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
				
			_gameAudio.PlayUiClick();
			FillUnitInfo( species );
		}

		private void OnUnitUpgraded( Species species )
		{
			FillUnitInfo( species );
		}

	}
}
