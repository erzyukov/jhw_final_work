namespace Game.Weapon
{
	using Game.Core;
	using Game.Fx;
	using Game.Units;
	using System;
	using UnityEngine;
	using Zenject;

	public enum ProjectileType
	{
		None,

		SniperBullet,
		Fireball,
		GrenadeCapsule,
	}

	[RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour, IPoolable<ProjectileArgs, IMemoryPool>, IDisposable
	{
		[Inject] protected IBattleEvents BattleEvents;

		[SerializeField] private ParticleFx _particles;

		protected IMemoryPool Pool;
		protected IUnitFacade Target;
		protected float Speed;
		protected float Damage;
		protected Rigidbody Rigidbody;
		protected Vector3 TargetPosition;
		protected ProjectileArgs Args;

		private void Awake()
		{
			Rigidbody = GetComponent<Rigidbody>();
		}

		private void FixedUpdate()
		{
			Move();

			if (transform.position == TargetPosition)
			{
				Target.TakeDamage(Damage);
				Dispose();
			}
		}
		
		private void OnTriggerEnter(Collider other)
		{
			if (Target.IsDead || other.transform != Target.Transform)
				return;

			Target.TakeDamage(Damage);
			Dispose();
		}

		protected virtual void Move()
		{
			TargetPosition = (Target == null || Target.IsDead) ? TargetPosition : Target.ModelRendererTransform.position;
			Vector3 newPosition = Vector3.MoveTowards(transform.position, TargetPosition, Speed * Time.fixedDeltaTime);
			Rigidbody.MovePosition(newPosition);
		}

		public void Dispose()
		{
			_particles?.Pause();
			Pool.Despawn(this);
		}

		public void OnDespawned()
		{
			Pool = null;
			Target = null;
			Speed = 0;
		}

		public virtual void OnSpawned(ProjectileArgs args, IMemoryPool pool)
		{
			Args = args;
			transform.position = args.StartPosition;
			Pool = pool;
			Target = args.Target;
			Speed = args.Speed;
			Damage = args.Damage;
			_particles?.Play();
		}
	}
}