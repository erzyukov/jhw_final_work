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
		void TakeDamage(float damage);
	}

	public class UnitHealth : ControllerBase, IUnitHealth, IInitializable
	{
		[Inject] private UnitGrade _unitGrade;

		private float _baseHealth;
		private float _health;

		public void Initialize()
		{
			_baseHealth = _unitGrade.Health;
			_health = _unitGrade.Health;
			HealthRate.Value = 1;
		}

		#region IUnitHealth

		public FloatReactiveProperty HealthRate { get; } = new FloatReactiveProperty();

		public ReactiveCommand Died { get; } = new ReactiveCommand();

		public void TakeDamage(float damage)
		{
			if (_health <= 0)
				return;

			_health = Mathf.Clamp(_health - damage, 0, _baseHealth);

			HealthRate.Value = _health / _baseHealth;

			Debug.LogWarning($"Unit health: {_health}");

			if (_health == 0)
				Died.Execute();
		}

		#endregion
	}
}