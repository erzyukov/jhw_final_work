namespace Game.Weapon
{
	using Zenject;

	public class Bullet : Projectile
	{
		public class Factory : PlaceholderFactory<ProjectileArgs, Bullet> { }
	}
}
