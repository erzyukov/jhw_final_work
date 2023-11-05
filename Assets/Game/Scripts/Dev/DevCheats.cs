namespace Game.Dev
{
	using Core;
	using Utilities;
	using Input;
	using UniRx;
	using UnityEngine.InputSystem;
	using VContainer;
	using VContainer.Unity;
	using System;
	using Game.Profiles;
	using UnityEngine;

	public class DevCheats : ControllerBase, IStartable
	{
		[Inject] IInputHandler _inputManager;
		[Inject] IGameLevel _gameLevel;
		[Inject] private GameProfile _profile;

		private Controls.DevCheatsActions Cheats => _inputManager.DevCheats;

		public void Start()
		{
			Subscribe(Cheats.NextWave, () => _gameLevel.GoToNextWave());
			
			Subscribe(Cheats.NextLevel, () => _gameLevel.GoToLevel(_profile.LevelNumber.Value + 1));

			Subscribe(Cheats.PrevLevel, () => _gameLevel.GoToLevel(_profile.LevelNumber.Value - 1));
		}

		void Subscribe(InputAction inputAction, Action action)
		=>
			inputAction.PerformedAsObservable()
				.Subscribe(_ => action())
				.AddTo(this);
	}
}
