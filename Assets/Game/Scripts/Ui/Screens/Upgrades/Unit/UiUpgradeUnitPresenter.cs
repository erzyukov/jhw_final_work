namespace Game.Ui
{
	using Game.Core;
	using Game.Profiles;
	using Game.Units;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Args = UiUpgradeUnitViewFactory.Args;

	public class UiUpgradeUnitPresenter : ControllerBase, IInitializable
	{
		[Inject] private Args					_args;
		[Inject] private IUiUpgradeUnitView		_view;
		[Inject] private IGameUpgrades			_gameUpgrades;
		[Inject] private GameProfile			_gameProfile;
		[Inject] private IUiUpgradeFlow			_flow;

		private Species Species => _args.Species;

		public void Initialize()
		{
			ControlSubscribe();

			_flow.SelectedUnit
				.Subscribe( OnUnitSelected )
				.AddTo( this );

			_gameUpgrades.Upgraded
				.Where( s => s == Species )
				.Subscribe( _ => OnUnitUpgraded() )
				.AddTo( this );

			_flow.SelectButtons.Add( Species, _view.SelectButton.gameObject );
			_flow.UpgradeButtons.Add( Species, _view.UpgradeButton.DefaultButton.gameObject );

			SetupView();
		}

		private void ControlSubscribe()
		{
			_view.SelectButtonClicked
				.Subscribe( _ => _flow.SelectedUnit.Value = _args.Species )
				.AddTo( this );

			_view.UpgradeButtonClicked
				.Subscribe( _ => _flow.UpgradeClicked.Execute( _args.Species ) )
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

		private void OnUnitUpgraded()
		{
			UpdateUnitParameters();
			_view.SetSelected( true );
		}

		private void UpdateUnitParameters()
		{
			int levelNumber		= _gameProfile.Units.Upgrades[Species].Value;
			int price			= _gameUpgrades.GetUpgradePrice( Species );

			_view.SetLevel( levelNumber );
			_view.SetPrice( price );
		}

		private void OnUnitSelected( Species species ) =>
			_view.SetSelected( Species == species );


	}
}
