namespace Game.Battle
{
	using Platoon;
	using Units;
	using UnityEngine;
	using UnityEngine.UIElements;

	public interface ITargetFinder
	{
		IUnit GetTarget(Vector2Int position, PlatoonFacade attackingPlatoon, PlatoonFacade defendingPlatoon);
	}

	public abstract class TargetFinder : ITargetFinder
	{
		protected Vector2Int PlatoonSize;

		public TargetFinder(Vector2Int platoonSize)
		{
			PlatoonSize = platoonSize;
		}

		public abstract IUnit GetTarget(Vector2Int position, PlatoonFacade attackingPlatoon, PlatoonFacade defendingPlatoon);

		protected Vector2Int GetOppositePosition(int xPosition, int yPosition) =>
			new Vector2Int(-xPosition + (PlatoonSize.x - 1), yPosition);

		protected bool IsFreeFronLine(Vector2Int position, PlatoonFacade attackingPlatoon)
		{
			if (position.y > 0)
			{
				for (int y = position.y - 1; y >= 0; y--)
				{
					Vector2Int cellPosition = new Vector2Int(position.x, y);
					PlatoonCell cell = attackingPlatoon.GetCell(cellPosition);

					if (cell.HasUnit)
						return false;
				}
			}

			return true;
		}
	}
}
