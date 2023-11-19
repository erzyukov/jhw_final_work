namespace Game.Installers
{
	using Game.Dev;
	using Game.Field;
	using Game.Level;
	using Zenject;

    public class LevelSceneInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			WebGLDebug.Log("[Project] LevelSceneInstaller: Configure");
			
			Container
				.BindInterfacesTo<HeroUnitSummoner>()
				.AsSingle();

			Container
				.BindInterfacesTo<UnitSpawner>()
				.AsSingle();

			Container
				.BindInterfacesTo<FieldHeroFacade>()
				.FromComponentInHierarchy()
				.AsSingle();
		}
	}
}