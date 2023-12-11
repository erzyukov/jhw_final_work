namespace Game.Weapon
{
	using Game.Units;
	using UnityEngine;

	public struct ProjectileData
	{
		public Vector3 StartPosition;
		public IUnitFacade Target;
		public float Speed;
		public float Damage;
	}
}