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
		ReactiveCommand Died { get; }
		string Name { get; }
		Species Species { get; }
		Transform Transform { get; }
		bool IsDead { get; }

		void SetViewParent(Transform parent);
		void SetDraggableActive(bool value);
		void TakeDamage(float damage);
		void EnableAttack();
		void ResetPosition();
		void Reset();
		void Destroy();
	}

	public class UnitFacade : IUnitFacade
	{
		[Inject] private Species _species;
		[Inject] private IUnitView _view;
		[Inject] private UnitConfig _config;
		[Inject] private IUnitHealth _health;
		[Inject] private IUnitFsm _fsm;
		[Inject] private IDraggable _draggable;

		#region IUnitFacade

		public ReactiveCommand Dragging => _draggable.Dragging;
		
		public ReactiveCommand Dropped => _draggable.Dropped;

		public ReactiveCommand Died => _health.Died;

		public string Name => _config.Title;

		public Species Species => _species;

		public Transform Transform => _view.Transform;

		public bool IsDead => _health.IsDead;

		public void SetViewParent(Transform parent) => _view.SetParent(parent);

		public void SetDraggableActive(bool value) => _draggable.SetActive(value);

		public void TakeDamage(float damage) => _health.TakeDamage(damage);

		public void EnableAttack() => _fsm.Transition(UnitState.SearchTarget);

		public void ResetPosition() => _view.ResetPosition();

		public void Reset() => _fsm.Transition(UnitState.Idle);

		public void Destroy() => _view.Destroy();

		#endregion

		public class Factory : PlaceholderFactory<Species, UnitFacade> {}
	}
}