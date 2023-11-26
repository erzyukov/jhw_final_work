namespace Game.Level
{
	using Game.Units;
	using Zenject;

	public interface IUnitSpawner
	{
		IUnitFacade SpawnEnemyUnit(Species species);
		IUnitFacade SpawnHeroUnit(Species species);
	}

	public class UnitSpawner : IUnitSpawner
	{
		[Inject] private UnitFacade.Factory _unitFactory;

		#region IUnitSpawner

		public IUnitFacade SpawnEnemyUnit(Species species)
		{
			return _unitFactory.Create(species);
		}

		public IUnitFacade SpawnHeroUnit(Species species)
		{
			IUnitFacade unit = _unitFactory.Create(species);

			return unit;
		}

		#endregion
	}
}