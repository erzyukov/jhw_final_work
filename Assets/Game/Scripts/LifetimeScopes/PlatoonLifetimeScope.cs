namespace Game.LifetimeScope
{
	using Level;
	using VContainer;
	using VContainer.Unity;

    public class PlatoonLifetimeScope : LifetimeScope
    {
		protected override void Configure(IContainerBuilder builder)
        {
			PlatoonView platoonView = GetComponent<PlatoonView>();
			builder.RegisterComponent(platoonView).AsImplementedInterfaces();

			builder.Register<Platoon>(Lifetime.Scoped).AsImplementedInterfaces();
			builder.Register<PlatoonBuilder>(Lifetime.Scoped).AsImplementedInterfaces();
		}
	}
}