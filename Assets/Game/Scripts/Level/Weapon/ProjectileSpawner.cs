namespace Game.Weapon
{
	using Game.Configs;
	using Game.Units;
	using UnityEngine;
	using Zenject;

	public interface IProjectileSpawner
	{
		Projectile Spawn(Vector3 position, ProjectileType type, IUnitFacade target, float damage);
	}

	public class ProjectileSpawner : IProjectileSpawner
	{
		[Inject] private Bullet.Factory _bulletFactory;
		[Inject] private Fireball.Factory _fireballFactory;
		[Inject] private WeaponsConfig _weaponsConfig;

		#region IProjectileSpawner

		public Projectile Spawn(Vector3 position, ProjectileType type, IUnitFacade target, float damage)
		{
			ProjectileData data = new ProjectileData()
			{
				StartPosition = position,
				Target = target,
				Speed = _weaponsConfig.BulletSpeed,
				Damage = damage
			};

			return Create(type, data);
		}

		#endregion

		private Projectile Create(ProjectileType type, ProjectileData data) => type switch
		{
			ProjectileType.SniperBullet => _bulletFactory.Create(data),
			ProjectileType.Fireball => _fireballFactory.Create(data),
			_ => null
		};
	}
}