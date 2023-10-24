namespace Game.Installers
{
	using Platoon;
	using UnityEngine;
	using VContainer;
	using VContainer.Unity;

    public class PlatoonInstaller : LifetimeScope
    {
		[SerializeField] private PlatoonCellView _platoonCellPrefab;

		protected override void Configure(IContainerBuilder builder)
        {
			builder.RegisterComponent(_platoonCellPrefab);

			PlatoonView platoonView = GetComponent<PlatoonView>();
			builder.RegisterComponent(platoonView).AsImplementedInterfaces();

			builder.Register<Platoon>(Lifetime.Scoped).AsImplementedInterfaces();
			builder.Register<PlatoonBuilder>(Lifetime.Scoped).AsImplementedInterfaces();
		}
	}
}