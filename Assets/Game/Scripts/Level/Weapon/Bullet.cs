namespace Game.Weapon
{
	using Game.Units;
	using System;
	using UnityEngine;
	using Zenject;

	[RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour, IPoolable<ProjectileData, IMemoryPool>, IDisposable
	{
		private IMemoryPool _pool;
		private IUnitFacade _target;
		private float _speed;
		private float _damage;
		private Rigidbody _rigidbody;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		private Vector3 _targetPosition;

		private void FixedUpdate()
		{
			_targetPosition = (_target == null || _target.IsDead) ? _targetPosition : _target.ModelRendererTransform.position;
			Vector3 newPosition = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.fixedDeltaTime);
			_rigidbody.MovePosition(newPosition);

			if (transform.position == _targetPosition)
			{
				_target.TakeDamage(_damage);
				Dispose();
			}
		}
		
		private void OnTriggerEnter(Collider other)
		{
			if (_target.IsDead || other.transform != _target.Transform)
				return;

			_target.TakeDamage(_damage);
			Dispose();
		}

		public void Dispose()
		{
			_pool.Despawn(this);
		}

		public void OnDespawned()
		{
			_pool = null;
			_target = null;
			_speed = 0;
		}

		public void OnSpawned(ProjectileData data, IMemoryPool pool)
		{
			transform.position = data.StartPosition;
			_pool = pool;
			_target = data.Target;
			_speed = data.Speed;
			_damage = data.Damage;
		}

		public class Factory : PlaceholderFactory<ProjectileData, Bullet> { }
	}
}