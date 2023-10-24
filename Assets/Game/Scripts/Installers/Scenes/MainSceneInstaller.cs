namespace Game.Installers
{
	using Game.Ui;
	using Game.Units;
	using UnityEngine;
	using VContainer;
	using VContainer.Unity;

    public class MainSceneInstaller : LifetimeScope
    {
		protected override void Configure(IContainerBuilder builder)
        {
			builder.RegisterComponentInHierarchy<HudUnitPanel>().AsImplementedInterfaces();

			builder.Register<UnitViewFactory>(Lifetime.Singleton);

			builder.RegisterComponentInHierarchy<Camera>();
		}
	}
}