namespace Game.Dev
{
	using Game.Core;
	using Game.Utilities;
	using Game.Input;
	using Game.Profiles;
	using UniRx;
	using UnityEngine.InputSystem;
	using System;
	using Zenject;
	using Game.Configs;
	using static UnityEngine.InputSystem.InputAction;

	public class DevCheats : ControllerBase, IInitializable
	{
		[Inject] private IInputHandler _inputManager;
		[Inject] private IGameLevel _gameLevel;
		[Inject] private GameProfile _profile;
		[Inject] private IGameCurrency _gameCurrency;
		[Inject] private DevConfig _devConfig;
		[Inject] private IScenesManager _scenesManager;
		[Inject] private IGameProfileManager _gameProfileManager;

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

			Subscribe(Cheats.AddSoftCurrency, (_) => _gameCurrency.AddSoftCurrency(SoftCurrencyCheatAmount));
			
			Subscribe(Cheats.AddSummonCurrency, (_) => _gameCurrency.AddSummonCurrency(SummonCurrencyCheatAmount));
			
			Subscribe(Cheats.RemoveGameSaves, (context) =>
			{
				if (context.performed && context.ReadValueAsButton())
					_scenesManager.ReloadGame(() => _gameProfileManager.Reset());
			});
		}

		void Subscribe(InputAction inputAction, Action<CallbackContext> action)
		=>
			inputAction.PerformedAsObservable()
				.Subscribe(context => action(context))
				.AddTo(this);
	}
}