namespace Game.Level
{
	using Game.Units;
	using Zenject;

	public interface IUnitSpawner
	{
		IUnitFacade SpawnEnemyUnit(Species species);
		IUnitFacade SpawnHeroUnit(Species species, int gradeIndex);
	}

	public class UnitSpawner : IUnitSpawner
	{
		[Inject] private UnitFacade.Factory _unitFactory;

		#region IUnitSpawner

		public IUnitFacade SpawnEnemyUnit(Species species)
		{
			return _unitFactory.Create(species, 0);
		}

		public IUnitFacade SpawnHeroUnit(Species species, int gradeIndex)
		{
			IUnitFacade unit = _unitFactory.Create(species, gradeIndex);

			return unit;
		}

		#endregion
	}
}