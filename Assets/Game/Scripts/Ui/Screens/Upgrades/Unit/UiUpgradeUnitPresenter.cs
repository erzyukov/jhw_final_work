namespace Game.Ui
{
	using Game.Core;
	using Game.Profiles;
	using Game.Units;
	using Game.Utilities;
	using System;
	using UnityEngine;
	using Zenject;
	using Args = UiUpgradeUnitViewFactory.Args;

	public class UiUpgradeUnitPresenter : ControllerBase, IInitializable
	{
		[Inject] private Args					_args;
		[Inject] private IUiUpgradeUnitView		_view;
		[Inject] private IGameUpgrades			_gameUpgrades;
		[Inject] private GameProfile			_gameProfile;

		public void Initialize()
		{

			SetupView();
		}
		private void SetupView()
		{
			int levelNumber		= _gameProfile.Units.Upgrades[_args.Species].Value;
			int price			= _gameUpgrades.GetUpgradePrice( _args.Species );
			var unit			= _args.Config;

			_view.SetIcon( unit.Icon );
			_view.SetLevel( levelNumber );
			_view.SetTitle( unit.Name );
			_view.SetPrice( price );
			
		}
	}
}
