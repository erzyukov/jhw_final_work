namespace Game.Core
{
	using UniRx;
	using UnityEngine;
	using Game.Units;
	using Game.Weapon;

	public struct DamageData
	{
		public float Amount;
		public IUnitFacade Target;
		public EDamageType Type;
		public float Range;
		public Vector3 Position;
		public ProjectileType ProjectileType;
	}

	public interface IBattleEvents
	{
		ReactiveCommand<DamageData> DamageApplyed { get; }
	}

	public class BattleEvents : IBattleEvents
	{
		public ReactiveCommand<DamageData> DamageApplyed { get; } = new();
	}
}
