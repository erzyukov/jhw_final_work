namespace Game.Dev
{
	using Game.Core;
	using Game.Utilities;
	using Game.Input;
	using Game.Profiles;
	using Game.Configs;
	using UniRx;
	using UnityEngine.InputSystem;
	using System;
	using Zenject;
	using static UnityEngine.InputSystem.InputAction;
    using UnityEngine;
	using Sirenix.Utilities;
	using Game.Units;

	public class DevCheats : ControllerBase, IInitializable
	{
		[Inject] private IInputHandler _inputManager;
		[Inject] private IGameLevel _gameLevel;
		[Inject] private GameProfile _profile;
		[Inject] private IGameCurrency _gameCurrency;
		[Inject] private DevConfig _devConfig;
		[Inject] private EnergyConfig _energyConfig;
		[Inject] private IScenesManager _scenesManager;
		[Inject] private IGameProfileManager _gameProfileManager;
		[Inject] private UnitsConfig _unitsConfig;

		const int SoftCurrencyCheatAmount = 10000;
		const int SummonCurrencyCheatAmount = 10;

		private Controls.DevCheatsActions Cheats => _inputManager.DevCheats;

		public void Initialize()
		{
			if (_devConfig.Build == DevConfig.BuildType.Debug)
				Cheats.Enable();
			else
				Cheats.Disable();

			Subscribe(Cheats.NextWave, (_) => _gameLevel.GoToNextWave());

			Subscribe(Cheats.NextLevel, (_) => _gameLevel.GoToLevel(_profile.LevelNumber.Value + 1));

			Subscribe(Cheats.PrevLevel, (_) => _gameLevel.GoToLevel(_profile.LevelNumber.Value - 1));

			Subscribe(Cheats.AddSoftCurrency, (_) => _gameCurrency.AddSoftCurrency(SoftCurrencyCheatAmount, SoftTransaction.None, "cheat"));
			
			Subscribe(Cheats.AddSummonCurrency, (_) => _gameCurrency.AddSummonCurrency(SummonCurrencyCheatAmount));
			
			Subscribe(Cheats.IncreaseHeroLevel, (_) => _profile.HeroLevel.Value++);

			Subscribe(Cheats.RemoveGameSaves, (context) =>
			{
				if (context.performed && context.ReadValueAsButton())
					_scenesManager.ReloadGame(() => _gameProfileManager.Reset());
			});

            Subscribe(Cheats.SpentEnergy, (_) => _profile.Energy.Amount.Value = Mathf.Max(_profile.Energy.Amount.Value - _energyConfig.LevelPrice, 0));
            Subscribe(Cheats.RestoreEnergy, (_) => _profile.Energy.Amount.Value = Mathf.Min(_profile.Energy.Amount.Value + _energyConfig.LevelPrice, _energyConfig.MaxEnery));

			Subscribe( Cheats.ResetUpgrades, ( _ ) =>
			{
				_profile.Units.Upgrades.Keys.ForEach( k =>
				{
					int defaultLevel = _unitsConfig.HeroDefaultSquad.Contains( k ) ? 1 : 0 ;
					_profile.Units.Upgrades[k].Value = defaultLevel;
				} );
				_gameProfileManager.Save();
			} );
        }

        void Subscribe(InputAction inputAction, Action<CallbackContext> action)
		=>
			inputAction.PerformedAsObservable()
				.Subscribe(context => action(context))
				.AddTo(this);
	}
}