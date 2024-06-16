namespace Game.Ui
{
	using Game.Core;
	using Game.Profiles;
	using Game.Units;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Args = UiUpgradeUnitViewFactory.Args;
	using Game.Configs;
	using Game.Managers;

	public class UiUpgradeUnitPresenter : ControllerBase, IInitializable
	{
		[Inject] private Args					_args;
		[Inject] private IUiUpgradeUnitView		_view;
		[Inject] private IGameUpgrades			_gameUpgrades;
		[Inject] private GameProfile			_gameProfile;
		[Inject] private IUiUpgradeFlow			_flow;
		[Inject] private UpgradesConfig			_upgradesConfig;
		[Inject] private IAdsManager			_adsManager;

		private Species Species		=> _args.Species;
		private int LevelNumber		=> _gameUpgrades.GetUnitLevel( Species );
		private bool IsBlocked		=> LevelNumber == 0;

		public void Initialize()
		{
			ControlSubscribe();

			Observable.Merge(
					_flow.SelectedUnit,
					_gameUpgrades.Upgraded
				)
				.Subscribe( OnUnitSelected )
				.AddTo( this );

			Observable.Merge(
					_gameUpgrades.Upgraded.Where( s => s == Species ).AsUnitObservable(),
					_gameProfile.HeroLevel.AsUnitObservable(),
					_gameProfile.SoftCurrency.AsUnitObservable()//,
					//_adsManager.IsRewardedAvailable.AsUnitObservable()
				)
				.Subscribe( _ => UpdateUnitParameters() )
				.AddTo( this );

			_flow.SelectButtons.Add( Species, _view.SelectButton.gameObject );
			_flow.UpgradeButtons.Add( Species, _view.UpgradeButton.DefaultButton.gameObject );

			SetupView();
		}

		private void ControlSubscribe()
		{
			Observable.Merge(
					_view.SelectButtonClicked,
					_view.UpgradeButtonClicked,
					_view.UnlockButtonClicked
				)
				.Subscribe( _ => _flow.SelectedUnit.Value = _args.Species )
				.AddTo( this );

			Observable.Merge(
					_view.UpgradeButtonClicked,
					_view.UnlockButtonClicked
				)
				.Subscribe( _ => _flow.UpgradeClicked.Execute( _args.Species ) )
				.AddTo( this );

			_view.AdsButtonClicked
				.Subscribe( _ => _adsManager.ShowRewardedVideo( ERewardedType.UnitUpgrade, new Rewarded( Species ) ) )
				.AddTo( this );

			_flow.SelectDisabled.ObserveAdd()
				.Where( s => s.Value == _args.Species )
				.Subscribe( _ => _view.SetSelectInteractable( false ) )
				.AddTo( this );

			_flow.SelectDisabled.ObserveRemove()
				.Where( s => s.Value == _args.Species )
				.Subscribe( _ => _view.SetSelectInteractable( true ) )
				.AddTo( this );

			_flow.UpgradeDisabled.ObserveAdd()
				.Where( s => s.Value == _args.Species )
				.Subscribe( _ => _view.SetUpgradeInteractable( false ) )
				.AddTo( this );

			_flow.UpgradeDisabled.ObserveRemove()
				.Where( s => s.Value == _args.Species )
				.Subscribe( _ => _view.SetUpgradeInteractable( true ) )
				.AddTo( this );
		}

		private void SetupView()
		{
			var unit			= _args.Config;

			_view.SetIcon( unit.Icon );
			_view.SetTitle( unit.Name );
			UpdateUnitParameters();
		}

		private void UpdateUnitParameters()
		{
			int price					= _gameUpgrades.GetUpgradePrice( Species );

			_view.SetLevel( LevelNumber );
			_view.SetPrice( price );
			_view.SetBlocked( IsBlocked );
			_view.SetLevelNumberActive( LevelNumber > 0 );

			bool isLocked				= _gameUpgrades.IsLockedByLevel( Species );
			_view.SetUnlockLevelActive( !isLocked );
			_view.SetLockedButtonActive( isLocked && LevelNumber == 0 );

			int unlockLevel				= _upgradesConfig.UnitsUpgrades[Species].UnlockHeroLevel;
			_view.SetUnlockLevel( unlockLevel );

			bool canUnlocked			= LevelNumber == 0 && isLocked == false;
			_view.SetUnlockButtonActive( canUnlocked );
			_view.SetUnlockPrice( price );

			int ccy						= _gameProfile.SoftCurrency.Value;
			bool isRewardAvailable		= _adsManager.IsRewardedAvailable.Value;
			bool isAdAcitve				= 
				(price > ccy)		&& 
				//isRewardAvailable	&& 
				LevelNumber != 0	&&
				LevelNumber < _gameProfile.HeroLevel.Value;

			_view.UpgradeButton.SetAdActive( isAdAcitve );
		}

		private void OnUnitSelected( Species species ) =>
			_view.SetSelected( Species == species );
	}
}
