namespace Game.Installers
{
	using Battle;
	using VContainer;
	using VContainer.Unity;

    public class LevelSceneInstaller : LifetimeScope
    {
		protected override void Configure(IContainerBuilder builder)
        {
			builder.Register<BattleSimulator>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<TargetFinderSelector>(Lifetime.Singleton).AsImplementedInterfaces();
		}
	}
}