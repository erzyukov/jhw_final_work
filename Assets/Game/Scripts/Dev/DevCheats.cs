namespace Game.Dev
{
	using Game.Core;
	using Game.Utilities;
	using Game.Input;
	using Game.Profiles;
	using Game.Units;
	using UniRx;
	using UnityEngine.InputSystem;
	using System;
	using Zenject;
	using UnityEngine;

	public class DevCheats : ControllerBase, IInitializable
	{
		[Inject] private IInputHandler _inputManager;
		[Inject] private IGameLevel _gameLevel;
		[Inject] private GameProfile _profile;
		[Inject] private UnitFacade.Factory _unitFactory;
		[Inject] private IGameCurrency _gameCurrency;

		const int SoftCurrencyCheatAmount = 10000;
		const int SummonCurrencyCheatAmount = 10;

		private Controls.DevCheatsActions Cheats => _inputManager.DevCheats;

		public void Initialize()
		{
			Subscribe(Cheats.NextWave, () => _gameLevel.GoToNextWave());

			Subscribe(Cheats.NextLevel, () => _gameLevel.GoToLevel(_profile.LevelNumber.Value + 1));

			Subscribe(Cheats.PrevLevel, () => _gameLevel.GoToLevel(_profile.LevelNumber.Value - 1));

			Subscribe(Cheats.BuyUnit, () =>
			{
				IUnitFacade unit = _unitFactory.Create(Species.HeroInfantryman);
				Debug.LogWarning($"Unit {unit.Species} bought!");
			});

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