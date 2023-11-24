namespace Game.Units
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using UnityEngine;

	public interface IUnitHealth
	{
		FloatReactiveProperty HealthRate { get; }
		ReactiveCommand Died { get; }

		bool IsDead { get; }

		void TakeDamage(float damage);
		void Reset();
	}

	public class UnitHealth : ControllerBase, IUnitHealth
	{
		[Inject] private UnitGrade _unitGrade;

		private float _baseHealth;
		private float _health;

		#region IUnitHealth

		public FloatReactiveProperty HealthRate { get; } = new FloatReactiveProperty();

		public ReactiveCommand Died { get; } = new ReactiveCommand();

		public bool IsDead => _health <= 0;

		public void TakeDamage(float damage)
		{
			if (_health <= 0)
				return;

			_health = Mathf.Clamp(_health - damage, 0, _baseHealth);

			HealthRate.Value = _health / _baseHealth;

			if (_health == 0)
				Died.Execute();
		}

		public void Reset()
		{
			_baseHealth = _unitGrade.Health;
			_health = _unitGrade.Health;
			HealthRate.Value = 1;
		}

		#endregion
	}
}