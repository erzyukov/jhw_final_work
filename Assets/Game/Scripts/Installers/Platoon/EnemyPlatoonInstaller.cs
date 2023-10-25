namespace Game.Installers
{
	using Platoon;
	using VContainer;
	using VContainer.Unity;

	public class EnemyPlatoonInstaller : PlatoonInstaller
	{
		protected override void Configure(IContainerBuilder builder)
		{
			base.Configure(builder);

			builder.Register<EnemyPlatoonController>(Lifetime.Scoped).AsImplementedInterfaces();

			builder.RegisterComponentInHierarchy<EnemyPlatoonFacade>().AsImplementedInterfaces();
		}
	}
}
