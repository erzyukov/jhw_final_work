namespace Game.Units
{
    using Game.Utilities;
    using Zenject;
    using UniRx;

    public interface IUnitEvents
    {
        ReactiveCommand Dying { get; }
        ReactiveCommand Died { get; }
    }

    public class UnitEvents : ControllerBase, IUnitEvents, IInitializable
    {
        [Inject] IUnitFsm _fsm;

        public void Initialize()
        {
            _fsm.StateChanged
                .Subscribe(OnStateChanged)
                .AddTo(this);
        }

        private void OnStateChanged(UnitState state)
        {
            switch (state)
            {
                case UnitState.Dying:
                    Dying.Execute();
                    break;
                
                case UnitState.Dead: 
                    Died.Execute(); 
                    break;
            }
        }

        #region IUnitEvents
        public ReactiveCommand Dying { get; } = new ReactiveCommand();
        public ReactiveCommand Died { get; } = new ReactiveCommand();
        #endregion
    }
}