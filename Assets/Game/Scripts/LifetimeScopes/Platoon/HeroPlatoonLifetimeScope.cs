namespace Game.LifetimeScope
{
	using Game.Units;
	using VContainer;
	using VContainer.Unity;

    public class HeroPlatoonLifetimeScope : PlatoonLifetimeScope
	{
        protected override void Configure(IContainerBuilder builder)
        {
			base.Configure(builder);

			builder.Register<HeroUnitCreator>(Lifetime.Scoped).AsImplementedInterfaces();
		}
	}
}
