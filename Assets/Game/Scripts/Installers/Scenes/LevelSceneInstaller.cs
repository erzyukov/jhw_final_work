namespace Game.Installers
{
	using Game.Dev;
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
		}
	}
}