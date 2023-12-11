namespace Game.Weapon
{
	using Game.Configs;
	using Game.Units;
	using UnityEngine;
	using Zenject;

	public interface IProjectileSpawner
	{
		public Bullet SpawnBullet(Vector3 position, IUnitFacade target, float damage);
	}

	public class ProjectileSpawner : IProjectileSpawner
	{
		[Inject] private Bullet.Factory _bulletFactory;
		[Inject] private WeaponsConfig _weaponsConfig;

		#region IProjectileSpawner

		public Bullet SpawnBullet(Vector3 position, IUnitFacade target, float damage)
		{
			ProjectileData data = new ProjectileData()
			{
				StartPosition = position,
				Target = target,
				Speed = _weaponsConfig.BulletSpeed,
				Damage = damage
			};

			return _bulletFactory.Create(data);
		}
		
		#endregion
	}
}