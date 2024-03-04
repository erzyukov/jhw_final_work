namespace Game.Units
{
	using Game.Configs;
	using Game.Utilities;
	using UnityEngine;
	using Zenject;

	public interface IUnitFacade
	{
		IUnitEvents Events { get; }

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
		void SetSupposedPower(int value);
		void ResetSupposedPower();
		void EnterBattle();
		void ResetPosition();
		void Reset();
		void Destroy();
	}

	public class UnitFacade : IUnitFacade
	{
		[Inject] private UnitCreateData _unitCreateData;
		[Inject] private IUnitData _unitData;
		[Inject] private IUnitView _view;
		[Inject] private UnitConfig _config;
		[Inject] private IUnitHealth _health;
		[Inject] private IUnitFsm _fsm;
		[Inject] private IUnitEvents _events;
		[Inject] private IUnitPosition _unitPosition;
		[Inject] private IDraggable _draggable;

		#region IUnitFacade

		public IUnitEvents Events => _events;

		public string Name => _config.TitleKey;

		public Species Species => _unitCreateData.Species;

		public int GradeIndex => _unitData.GradeIndex;
		
		public int Power => _unitData.Power.Value;

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

		public void SetSupposedPower(int value) =>
			_unitData.SupposedPower.Value = value;

		public void ResetSupposedPower() =>
			_unitData.SupposedPower.Value = 0;

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