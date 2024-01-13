namespace Game.Units
{
    using Game.Utilities;
    using Zenject;
    using UniRx;

    public interface IUnitEvents
    {
        ReactiveCommand Dying { get; }
        ReactiveCommand Died { get; }
		ReactiveCommand Dragging { get; }
		ReactiveCommand Dropped { get; }
		ReactiveCommand PointerDowned { get; }
		ReactiveCommand PointerUped { get; }
		ReactiveCommand MergeInitiated { get; }
		ReactiveCommand MergeCanceled { get; }
		ReactiveCommand Attacking { get; }
	}

	public class UnitEvents : ControllerBase, IUnitEvents, IInitializable
    {
        [Inject] private IUnitFsm _fsm;
		[Inject] private IDraggable _draggable;
		[Inject] private IUnitView _view;
		[Inject] private IUnitAttacker _attacker;

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
		public ReactiveCommand Dropped => _draggable.Dropped;
		public ReactiveCommand PointerDowned => _draggable.PointerDowned;
		public ReactiveCommand PointerUped => _draggable.PointerUped;
		public ReactiveCommand MergeInitiated => _view.MergeInitiated;
		public ReactiveCommand MergeCanceled => _view.MergeCanceled;
		public ReactiveCommand Attacking => _attacker.Attacking;
		#endregion
	}
}