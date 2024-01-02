namespace Game.Dev
{
    using Game.Utilities;
    using Game.Input;
    using UniRx;
    using UnityEngine.InputSystem;
    using System;
    using Zenject;
    using static UnityEngine.InputSystem.InputAction;
    using Game.Field;
	using System.Linq;
	using Game.Units;

	public class LevelDevCheats : ControllerBase, IInitializable
    {
        [Inject] private IInputHandler _inputManager;
        [Inject] private IFieldEnemyFacade _fieldEnemyFacade;

        private Controls.DevCheatsActions Cheats => _inputManager.DevCheats;

        public void Initialize()
        {
            Subscribe(Cheats.KillEnemyUnit, (_) =>
            {
				IUnitFacade unit = _fieldEnemyFacade.Units.Where(unit => unit.IsDead == false).FirstOrDefault();
				if (unit != null)
					unit.TakeDamage(9999999);
            });

            Subscribe(Cheats.KillAllEnemies, (_) =>
            {
                foreach (var ememy in _fieldEnemyFacade.Units)
                    ememy.TakeDamage(9999999);
            });
        }

        void Subscribe(InputAction inputAction, Action<CallbackContext> action)
        =>
            inputAction.PerformedAsObservable()
                .Subscribe(context => action(context))
                .AddTo(this);
    }
}