namespace Game.Configs
{
	using Game.Weapon;
	using UnityEngine;

	[CreateAssetMenu(fileName = "Weapons", menuName = "Configs/Weapons", order = (int)Config.Weapons)]
	public class WeaponsConfig : ScriptableObject
	{
		[Header("Bullet / Technical")]
		[SerializeField] private Bullet _bulletPrefab;
		[SerializeField] private int _bulletPoolSize;
		[SerializeField] private float _bulletSpeed;

		public Bullet BulletPrefab => _bulletPrefab;
		public int BulletPoolSize => _bulletPoolSize;
		public float BulletSpeed => _bulletSpeed;
	}
}