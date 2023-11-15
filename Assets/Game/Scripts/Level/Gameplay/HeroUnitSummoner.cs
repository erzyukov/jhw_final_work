namespace Game.Level
{
	using Game.Units;
	using Game.Utilities;
	using Zenject;

	public interface IHeroUnitSummoner
	{
		void Summon();
	}

	public class HeroUnitSummoner : ControllerBase, IHeroUnitSummoner, IInitializable
	{
		[Inject] private UnitFacade.Factory _unitFactory;

		public void Initialize()
		{
			
		}

		public void Summon()
		{
			_unitFactory.Create(Species.HeroInfantryman);
		}
	}
}
