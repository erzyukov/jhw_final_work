namespace Game.Battle
{
	using Core;
	using Platoon;
	using Units;
	using Utilities;
	using VContainer.Unity;
	using UnityEngine;
	using Kind = Units.Unit.Kind;
	using VContainer;

	public interface IBattleSimulator
	{
		void RegisterHeroPlatoonFacade(HeroPlatoonFacade heroPlatoon);
		void RegisterEnemyPlatoonFacade(EnemyPlatoonFacade enemyPlatoon);
	}

	public class BattleSimulator : ControllerBase, IStartable, ITickable, IBattleSimulator
	{
		[Inject] private IGameLevel _gameLevel;
		[Inject] private ITargetFinderSelector _targetFinderSelector;

		private HeroPlatoonFacade _heroPlatoon;
		private EnemyPlatoonFacade _enemyPlatoon;

		public void Start()
		{
		}

		public void Tick()
		{
			if (_heroPlatoon == null ||  _enemyPlatoon == null)
				return;

			SimulatePlatoonAttack(_heroPlatoon, _enemyPlatoon);
			SimulatePlatoonAttack(_enemyPlatoon, _heroPlatoon);
		}

		public void RegisterHeroPlatoonFacade(HeroPlatoonFacade heroPlatoon) =>
			_heroPlatoon = heroPlatoon;

		public void RegisterEnemyPlatoonFacade(EnemyPlatoonFacade enemyPlatoon) =>
			_enemyPlatoon = enemyPlatoon;

		private void SimulatePlatoonAttack(PlatoonFacade attackingPlatoon, PlatoonFacade defendingPlatoon)
		{
			foreach (var unit in attackingPlatoon.ReadyUnits)
			{
				IUnit targetUnit = FindTarget(unit.GetKind(), attackingPlatoon.GetCell(unit).Position, attackingPlatoon, defendingPlatoon);

				if (targetUnit != null)
					unit.Shoot(targetUnit);
			}
		}

		// TODO: refact: use strategy pattern
		private IUnit FindTarget(Kind attackerType, Vector2Int position, PlatoonFacade attackingPlatoon, PlatoonFacade defendingPlatoon)
		{
			ITargetFinder targetFinder = _targetFinderSelector.GetTargetFinder(attackerType);

			return targetFinder.GetTarget(position, attackingPlatoon, defendingPlatoon);
		}

		/*
		private IUnit GetHeavyTankTarget(Vector2Int position, PlatoonFacade defendingPlatoon)
		{
			int[] yPositions = { 0, 1, 2 };
            for (int y = 0; y < yPositions.Length; y++)
            {
				Vector2Int targetPosition = GetOppositePosition(position, y);
				PlatoonCell cell = defendingPlatoon.GetCell(targetPosition);
                
				if (cell == null)
					continue;

				if (cell.HasUnit)
					return cell.Unit;
			}

			return default;
        }

		private IUnit GetLightTankTarget(Vector2Int position, PlatoonFacade defendingPlatoon)
		{

			return default;
		}

		private Vector2Int GetOppositePosition(Vector2Int position, int yPosition) =>
			new Vector2Int(-position.x + (_gameLevel.PlatoonSize.x - 1), yPosition);
		*/
	}
}
