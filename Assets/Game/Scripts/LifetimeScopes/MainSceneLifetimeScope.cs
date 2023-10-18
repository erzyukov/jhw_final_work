namespace Game.LifetimeScope
{
	using Game.Ui;
	using Game.Units;
	using VContainer;
	using VContainer.Unity;

    public class MainSceneLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
			builder.RegisterComponentInHierarchy<HudUnitPanel>().AsImplementedInterfaces();

			builder.Register<UnitViewFactory>(Lifetime.Singleton);
			builder.Register<UnitCreator>(Lifetime.Singleton).AsImplementedInterfaces();
		}
	}
}