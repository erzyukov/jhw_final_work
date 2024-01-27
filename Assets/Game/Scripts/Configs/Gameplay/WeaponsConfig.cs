namespace Game.Configs
{
	using Game.Weapon;
	using Sirenix.OdinInspector;
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "Weapons", menuName = "Configs/Weapons", order = (int)Config.Weapons)]
	public class WeaponsConfig : SerializedScriptableObject
	{
		[Header("Bullet / Technical")]
		[SerializeField] private Dictionary<ProjectileType, Projectile> _projectilePrefabs = new Dictionary<ProjectileType, Projectile>();
		[SerializeField] private int _bulletPoolSize;
		[SerializeField] private float _bulletSpeed;

		public int BulletPoolSize => _bulletPoolSize;
		public float BulletSpeed => _bulletSpeed;

		public Projectile GetProjectile(ProjectileType type)
		{
			if (_projectilePrefabs.TryGetValue(type, out Projectile projectile))
				return projectile;

			return null;
		}
	}
}