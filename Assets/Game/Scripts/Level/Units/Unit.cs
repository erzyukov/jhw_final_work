namespace Game.Units
{
	using Game.Configs;
	using Game.Utilities;
	using UniRx;
	using UnityEngine;

	public interface IUnit
	{
		FloatReactiveProperty Health { get; }
		bool IsReadyToShoot { get; }
		float TimeToShoot { get; }
		void Shoot(IUnit target);
		void TakeDamage(float amount);
		void SetViewParent(Transform tranform);
		Unit.Kind GetKind();
		void SetIgnoreRaycast(bool value);
		void UpdateView();
	}

	public class Unit : IUnit
	{
		protected Kind _kind;
		protected IUnitView _unitView;
		protected UnitConfig _config;
		private ITimer _shootTimer;

		public Unit(Kind kind, IUnitView unitView, UnitConfig config)
		{
			_kind = kind;
			_unitView = unitView;
			_config = config;
			_shootTimer = new Timer();

			Health.Value = _config.Health;
		}

		public FloatReactiveProperty Health { get; } = new FloatReactiveProperty();
		public bool IsReadyToShoot => _shootTimer.IsReady;
		public float TimeToShoot => _shootTimer.Remained;

		public void Shoot(IUnit target)
		{
			_shootTimer.Set(_config.ShootDelay);
			target.TakeDamage(_config.Damage);
			//Debug.LogWarning($">> {_config.Title} shooted!");
		}

		public void TakeDamage(float amount)
		{
			Health.Value = Mathf.Max(0, Health.Value - amount);
			_unitView.SetHealthRatio(Health.Value / _config.Health);

			//Debug.LogWarning($">> {_config.Title} recieved {amount} damage!");
		}

		public void SetViewParent(Transform transform) =>
			_unitView.SetParent(transform);

		public Kind GetKind() => 
			_kind;

		public void SetIgnoreRaycast(bool value) => 
			_unitView.SetIgnoreRaycast(value);

		public void UpdateView()
		{
			_unitView.SetCooldownRatio(_shootTimer.Remained / _config.ShootDelay);
		}

		public enum Kind
		{
			HeavyTank,
			LightTank,
			Howitzer,
			Support,
		}
	}
}