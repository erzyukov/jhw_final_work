namespace Game.Weapon
{
	using Zenject;

	public class Acidbolt : Projectile
	{
		public class Factory : PlaceholderFactory<ProjectileArgs, Acidbolt> { }
	}
}
