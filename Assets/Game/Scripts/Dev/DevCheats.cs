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
	using UnityEngine;

	public class DevCheats : ControllerBase, IStartable
	{
		[Inject] IInputHandler _inputManager;
		[Inject] IGameLevel _gameLevel;

		private Controls.DevCheatsActions Cheats => _inputManager.DevCheats;

		public void Start()
		{
			Subscribe(Cheats.NextWave, () => Debug.LogWarning("Go To Next Wave"));
			
			Subscribe(Cheats.NextLevel, () => Debug.LogWarning("Go To Next Level"));

			Subscribe(Cheats.PrevLevel, () => Debug.LogWarning("Go To Previous Level"));
		}

		void Subscribe(InputAction inputAction, Action action)
		=>
			inputAction.PerformedAsObservable()
				.Subscribe(_ => action())
				.AddTo(this);
	}
}
