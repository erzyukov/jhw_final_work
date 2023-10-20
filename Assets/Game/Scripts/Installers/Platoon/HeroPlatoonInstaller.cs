namespace Game.Installers
{
	using Game.Units;
	using VContainer;

    public class HeroPlatoonInstaller : PlatoonInstaller
	{
        protected override void Configure(IContainerBuilder builder)
        {
			base.Configure(builder);

			builder.Register<HeroUnitCreator>(Lifetime.Scoped).AsImplementedInterfaces();
		}
	}
}
