namespace Game.Configs
{
	using Game.Weapon;
	using Sirenix.OdinInspector;
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	[Serializable]
	public struct ProjectileConfig
	{
		public float Speed;
		public float Height;
		public float DamageRange;
		public Projectile Prefab;
	}

	[CreateAssetMenu(fileName = "Weapons", menuName = "Configs/Weapons", order = (int)Config.Weapons)]
	public class WeaponsConfig : SerializedScriptableObject
	{
		[Header("Projectile / Technical")]
		public Dictionary<ProjectileType, ProjectileConfig> Projectile = new Dictionary<ProjectileType, ProjectileConfig>();
		public int ProjectilePoolSize;

		public Projectile GetProjectilePrefab(ProjectileType type)
		{
			if (Projectile.TryGetValue(type, out ProjectileConfig projectile))
				return projectile.Prefab;

			return null;
		}

		public ProjectileConfig? GetProjectile(ProjectileType type)
		{
			if (Projectile.TryGetValue(type, out ProjectileConfig projectile))
				return projectile;

			return null;
		}
	}
}