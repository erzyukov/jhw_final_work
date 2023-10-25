namespace Game.Battle
{
	using Platoon;
	using Units;
	using UnityEngine;

	public class HowitzerTargetFinder : TargetFinder
	{
		public HowitzerTargetFinder(Vector2Int platoonSize) : base(platoonSize) { }

		public override IUnit GetTarget(Vector2Int position, PlatoonFacade attackingPlatoon, PlatoonFacade defendingPlatoon)
		{
			for (int y = 0; y < PlatoonSize.y - 1; y++)
			{
				Vector2Int targetPosition = GetOppositePosition(position.x, y);
				PlatoonCell cell = defendingPlatoon.GetCell(targetPosition);

				if (cell == null)
					continue;

				if (cell.HasUnit)
					return cell.Unit;
			}

			return default;
		}
	}
}