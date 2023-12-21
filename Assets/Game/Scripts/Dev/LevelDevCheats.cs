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

    public class LevelDevCheats : ControllerBase, IInitializable
    {
        [Inject] private IInputHandler _inputManager;
        [Inject] private IFieldEnemyFacade _fieldEnemyFacade;

        private Controls.DevCheatsActions Cheats => _inputManager.DevCheats;

        public void Initialize()
        {
            Subscribe(Cheats.KillEnemyUnit, (_) =>
            {
                if (_fieldEnemyFacade.Units.Count > 0)
                    _fieldEnemyFacade.Units[0].TakeDamage(9999999);
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