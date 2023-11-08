namespace Game.Input
{
	using System;
	using UniRx;
	using UnityEngine.InputSystem;
	using Zenject;

	public interface IInputHandler
	{
		Controls.DevCheatsActions DevCheats { get; }
	}

	public class InputHandler : IInputHandler, IInitializable
	{
		private Controls _controls;

		public Controls.DevCheatsActions DevCheats { get; private set; }

		public void Initialize()
		{
			_controls = new Controls();
			DevCheats = _controls.DevCheats;

#if UNITY_EDITOR
			DevCheats.Enable();
#endif
		}
	}

	// https://github.com/neuecc/UniRx/issues/481
	public static class UniRxExtensions
	{
		public static IObservable<InputAction.CallbackContext> PerformedAsObservable(this InputAction action)
		=>
			Observable.FromEvent<InputAction.CallbackContext>(
				h => action.performed += h,
				h => action.performed -= h
			);
	}
}