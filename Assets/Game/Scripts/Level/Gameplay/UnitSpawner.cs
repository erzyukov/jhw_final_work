namespace Game.Level
{
	using Game.Units;
	using Zenject;

	public interface IUnitSpawner
	{
		IUnitFacade SpawnHeroUnit(Species species);
	}

	public class UnitSpawner : IUnitSpawner
	{
		[Inject] private UnitFacade.Factory _unitFactory;

		#region IUnitSpawner

		public IUnitFacade SpawnHeroUnit(Species species)
		{
			return _unitFactory.Create(species);
		}

		#endregion
	}
}