namespace Game.Units
{
    using Game.Utilities;
    using Zenject;
    using UniRx;
	using UnityEngine;

	public interface IUnitEvents
    {
        ReactiveCommand Dying { get; }
        ReactiveCommand Died { get; }
		ReactiveCommand Dragging { get; }
		ReactiveCommand<Vector2> Drag { get; }
		ReactiveCommand Dropped { get; }
		ReactiveCommand PointerDowned { get; }
		ReactiveCommand PointerUped { get; }
		ReactiveCommand Attacking { get; }
		ReactiveCommand<float> DamageReceived { get; }
	}

	public class UnitEvents : ControllerBase, IUnitEvents, IInitializable
    {
        [Inject] private IUnitFsm _fsm;
		[Inject] private IDraggable _draggable;
		[Inject] private IUnitAttacker _attacker;
		[Inject] private IUnitHealth _unitHealth;

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
		public ReactiveCommand Dragging => _draggable.Dragging;
		public ReactiveCommand<Vector2> Drag => _draggable.Drag;
		public ReactiveCommand Dropped => _draggable.Dropped;
		public ReactiveCommand PointerDowned => _draggable.PointerDowned;
		public ReactiveCommand PointerUped => _draggable.PointerUped;
		public ReactiveCommand Attacking => _attacker.Attacking;
		public ReactiveCommand<float> DamageReceived => _unitHealth.DamageReceived;
		#endregion
	}
}