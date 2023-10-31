namespace Game.Installers
{
	using Core;
	using UnityEngine;
	using VContainer;
	using VContainer.Unity;

    public class MainSceneInstaller : LifetimeScope
    {
		protected override void Configure(IContainerBuilder builder)
        {
			//builder.RegisterComponentInHierarchy<HudUnitPanel>().AsImplementedInterfaces();

			//builder.Register<UnitViewFactory>(Lifetime.Singleton);

			builder.Register<GameLevel>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.RegisterComponentInHierarchy<Camera>();

			//DevCheats devCheats = new DevCheats();
			//builder.RegisterInstance(devCheats);

		}
	}
}