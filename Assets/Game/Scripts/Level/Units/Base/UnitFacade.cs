namespace Game.Units
{
	using Game.Configs;
	using Game.Utilities;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public interface IUnitFacade
	{
		ReactiveCommand Dragging { get; }
		ReactiveCommand Dropped { get; }
		ReactiveCommand PointerDowned { get; }
		ReactiveCommand PointerUped { get; }
		ReactiveCommand MergeInitiated { get; }
		ReactiveCommand MergeCanceled { get; }
		ReactiveCommand Dying { get; }
        ReactiveCommand Died { get; }

        string Name { get; }
		Species Species { get; }
		int GradeIndex { get; }
		Transform Transform { get; }
		Transform ModelRendererTransform { get; }
		bool IsDead { get; }
		int Power { get; }

		void SetViewParent(Transform parent, bool worldPositionStays = false);
		void SetActive(bool value);
		void SetDraggableActive(bool value);
		void TakeDamage(float damage);
		void EnterBattle();
		void ResetPosition();
		void Reset();
		void Destroy();
	}

	public class UnitFacade : IUnitFacade
	{
		[Inject] private UnitCreateData _unitCreateData;
		[Inject] private IUnitView _view;
		[Inject] private UnitConfig _config;
		[Inject] private IUnitHealth _health;
		[Inject] private IUnitFsm _fsm;
		[Inject] private IUnitEvents _events;
		[Inject] private IDraggable _draggable;
		[Inject] private IUnitPosition _unitPosition;

		#region IUnitFacade

		public ReactiveCommand Dragging => _draggable.Dragging;
		
		public ReactiveCommand Dropped => _draggable.Dropped;

		public ReactiveCommand PointerDowned => _draggable.PointerDowned;

		public ReactiveCommand PointerUped => _draggable.PointerUped;

		public ReactiveCommand MergeInitiated => _view.MergeInitiated;

		public ReactiveCommand MergeCanceled => _view.MergeCanceled;

        public ReactiveCommand Dying => _events.Dying;
        
        public ReactiveCommand Died => _events.Died;

		public string Name => _config.TitleKey;

		public Species Species => _unitCreateData.Species;

		public int GradeIndex => _unitCreateData.GradeIndex;
		
		public int Power => _unitCreateData.Power;

		public Transform Transform => (_view != null) ? _view.Transform : null;

		public Transform ModelRendererTransform => _view.ModelRendererTransform;

		public bool IsDead => _health.IsDead;

		public void SetViewParent(Transform parent, bool worldPositionStays = false) => 
			_view.SetParent(parent, worldPositionStays);

		public void SetActive(bool value) =>
			_view.SetActive(value);

		public void SetDraggableActive(bool value) => 
			_draggable.SetActive(value);

		public void TakeDamage(float damage) => 
			_health.TakeDamage(damage);

		public void EnterBattle() =>
			_fsm.EnterBattle();

		public void ResetPosition() =>
			_unitPosition.ResetPosition();

		public void Reset() => 
			_fsm.Reset();

		public void Destroy() => 
			_view.Destroy();

		#endregion

		public class Factory : PlaceholderFactory<UnitCreateData, UnitFacade> {}
	}
}