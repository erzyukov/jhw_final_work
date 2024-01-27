namespace Game.Weapon
{
	using Zenject;

	public class Fireball : Projectile
	{
		public class Factory : PlaceholderFactory<ProjectileData, Fireball> { }
	}
}
