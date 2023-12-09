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

	public class DevCheats : ControllerBase, IInitializable
	{
		[Inject] private IInputHandler _inputManager;
		[Inject] private IGameLevel _gameLevel;
		[Inject] private GameProfile _profile;
		[Inject] private IGameCurrency _gameCurrency;
		[Inject] DevConfig _devConfig;

		const int SoftCurrencyCheatAmount = 10000;
		const int SummonCurrencyCheatAmount = 10;

		private Controls.DevCheatsActions Cheats => _inputManager.DevCheats;

		public void Initialize()
		{
			if (_devConfig.Build == DevConfig.BuildType.Debug)
				Cheats.Enable();
			else
				Cheats.Disable();

			Subscribe(Cheats.NextWave, () => _gameLevel.GoToNextWave());

			Subscribe(Cheats.NextLevel, () => _gameLevel.GoToLevel(_profile.LevelNumber.Value + 1));

			Subscribe(Cheats.PrevLevel, () => _gameLevel.GoToLevel(_profile.LevelNumber.Value - 1));

			Subscribe(Cheats.AddSoftCurrency, () => _gameCurrency.AddSoftCurrency(SoftCurrencyCheatAmount));
			
			Subscribe(Cheats.AddSummonCurrency, () => _gameCurrency.AddSummonCurrency(SummonCurrencyCheatAmount));
		}

		void Subscribe(InputAction inputAction, Action action)
		=>
			inputAction.PerformedAsObservable()
				.Subscribe(_ => action())
				.AddTo(this);
	}
}