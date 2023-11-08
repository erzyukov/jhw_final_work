namespace Game.Platoon
{
	using Units;
	using Battle;
	using System.Collections.Generic;
	using UnityEngine;
	using System.Linq;
	using Zenject;

	public interface IPlatoonFacade
	{
		List<IUnit> Units { get; }
		List<IUnit> ReadyUnits { get; }
		PlatoonCell GetCell(Vector2Int position);
		PlatoonCell GetCell(IUnit unit);
	}

	public abstract class PlatoonFacade : MonoBehaviour, IPlatoonFacade
	{
		[Inject] protected IBattleSimulator BattleSimulator;
		[Inject] private IPlatoon _platoon;

		public List<IUnit> Units => _platoon.Units;

		public List<IUnit> ReadyUnits => _platoon.Units.Where(unit => unit.IsReadyToShoot).ToList();

		public PlatoonCell GetCell(Vector2Int position) => _platoon.GetCell(position);
		public PlatoonCell GetCell(IUnit unit) => _platoon.GetCell(unit);
	}
}
