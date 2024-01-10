namespace Game.Units
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using UnityEngine;
	using Game.Configs;

	public interface IUnitHealth
	{
		FloatReactiveProperty HealthRatio { get; }
		ReactiveCommand Died { get; }

		bool IsDead { get; }

		void TakeDamage(float damage);
		void Reset();
	}

	public class UnitHealth : ControllerBase, IUnitHealth
	{
		[Inject] private IUnitData _unitData;
		[Inject] private UnitConfig _unitConfig;

		private float _baseHealth;
		private float _health;

		#region IUnitHealth

		public FloatReactiveProperty HealthRatio { get; } = new FloatReactiveProperty();

		public ReactiveCommand Died { get; } = new ReactiveCommand();

		public bool IsDead => _health <= 0;

		public void TakeDamage(float damage)
		{
			if (IsDead)
				return;

			_health = Mathf.Clamp(_health - damage, 0, _baseHealth);

			HealthRatio.Value = _health / _baseHealth;

			if (_health == 0)
				Died.Execute();
		}

		public void Reset()
		{
			_baseHealth = _unitConfig.Health + _unitConfig.HealthPowerMultiplier * _unitData.Power.Value;
			_health = _baseHealth;
			HealthRatio.Value = 1;
		}

		#endregion
	}
}