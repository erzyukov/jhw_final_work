namespace Game.Level
{
	using Game.Units;
	using Zenject;

	public interface IUnitSpawner
	{
		IUnitFacade SpawnEnemyUnit(Species species, int gradeIndex, int power);
		IUnitFacade SpawnHeroUnit(Species species, int gradeIndex, int power);
	}

	public class UnitSpawner : IUnitSpawner
	{
		[Inject] private UnitFacade.Factory _unitFactory;

		#region IUnitSpawner

		public IUnitFacade SpawnEnemyUnit(Species species, int gradeIndex, int power)
		{
			return _unitFactory.Create(new UnitData { GradeIndex = gradeIndex, Power = power, Species = species});
		}

		public IUnitFacade SpawnHeroUnit(Species species, int gradeIndex, int power)
		{
			return _unitFactory.Create(new UnitData { GradeIndex = gradeIndex, Power = power, Species = species });
		}

		#endregion
	}
}