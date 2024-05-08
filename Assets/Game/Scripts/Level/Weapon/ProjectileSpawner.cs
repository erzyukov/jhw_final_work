namespace Game.Weapon
{
	using Game.Configs;
	using Game.Units;
	using UnityEngine;
	using Zenject;

	public struct ProjectileArgs
	{
		public Vector3		StartPosition;
		public IUnitFacade	Target;
		public float		Speed;
		public float		Damage;
		public float		Height;
		public float		MaxDistance;
		public float		DamageRange;
		public ProjectileType Type;
	}

	public interface IProjectileSpawner
	{
		Projectile Spawn(Vector3 position, ProjectileType type, IUnitFacade target, float damage, float distance);
	}

	public class ProjectileSpawner : IProjectileSpawner
	{
		[Inject] private Bullet.Factory _bulletFactory;
		[Inject] private Fireball.Factory _fireballFactory;
		[Inject] private GrenadeCapsule.Factory _grenadeCapsuleFactory;
		[Inject] private WeaponsConfig _weaponsConfig;

		#region IProjectileSpawner

		public Projectile Spawn(Vector3 position, ProjectileType type, IUnitFacade target, float damage, float distance)
		{
			var projectile = _weaponsConfig.Projectile[type];

			ProjectileArgs data = new ProjectileArgs()
			{
				StartPosition		= position,
				Target				= target,
				Speed				= projectile.Speed,
				Damage				= damage,
				Height				= projectile.Height,
				MaxDistance			= distance,
				DamageRange			= projectile.DamageRange,
				Type				= type,
			};

			return Create(type, data);
		}

		#endregion

		private Projectile Create(ProjectileType type, ProjectileArgs data) => type switch
		{
			ProjectileType.SniperBullet => _bulletFactory.Create(data),
			ProjectileType.Fireball => _fireballFactory.Create(data),
			ProjectileType.GrenadeCapsule => _grenadeCapsuleFactory.Create(data),
			_ => null
		};
	}
}