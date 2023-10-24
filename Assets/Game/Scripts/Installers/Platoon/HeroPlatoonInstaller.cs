namespace Game.Installers
{
	using Platoon;
	using UnityEngine;
	using VContainer;

    public class HeroPlatoonInstaller : PlatoonInstaller
	{
        protected override void Configure(IContainerBuilder builder)
        {
			base.Configure(builder);

			builder.Register<HeroPlatoonController>(Lifetime.Scoped).AsImplementedInterfaces();
		}
	}
}
