namespace Game.Battle
{
	using Game.Utilities;
	using Platoon;
	using System.Linq;
	using Units;
	using UnityEngine;

	public class LightTankTargetFinder : TargetFinder
	{
		public LightTankTargetFinder(Vector2Int platoonSize) : base(platoonSize) { }

		public override IUnit GetTarget(Vector2Int position, PlatoonFacade attackingPlatoon, PlatoonFacade defendingPlatoon)
		{
			if (position.y > 1)
				return default;

			int[] xPositionsDelta = { 0, -1, 1 };
			int[] xPositions = xPositionsDelta
				.Select(delta => Mathf.Clamp(position.x + delta, 0, PlatoonSize.x - 1))
				.Distinct().ToArray();

			for (int i = 0; i < xPositions.Length; i++)
			{
				if (IsFreeFronLine(position.WithX(xPositions[i]), attackingPlatoon) == false)
					continue;

				Vector2Int targetPosition = GetOppositePosition(xPositions[i], 0);
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