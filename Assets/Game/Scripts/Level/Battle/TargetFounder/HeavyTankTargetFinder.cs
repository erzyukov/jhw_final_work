namespace Game.Battle
{
	using Platoon;
	using Units;
	using UnityEngine;

	public class HeavyTankTargetFinder : TargetFinder
	{
		public HeavyTankTargetFinder(Vector2Int platoonSize) : base(platoonSize) {}

		public override IUnit GetTarget(Vector2Int position, PlatoonFacade attackingPlatoon, PlatoonFacade defendingPlatoon)
		{
			int[] yPositions = { 0, 1, 2 };

			if (IsFreeFronLine(position, attackingPlatoon) == false)
				return default;

            for (int i = 0; i < yPositions.Length - position.y; i++)
			{
				Vector2Int targetPosition = GetOppositePosition(position.x, yPositions[i]);
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
