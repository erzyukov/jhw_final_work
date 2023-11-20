namespace Game.Units
{
	using Game.Configs;
	using UnityEngine;
	using Zenject;

	public class UnitInstaller : Installer<UnitInstaller>
	{
		[Inject] private Species _species;
		[Inject] private UnitsConfig _unitsConfig;

		public override void InstallBindings()
		{
			Debug.LogWarning($"UnitInstaller: InstallBindings");

			Container
				.BindInstance(_species);

			Container
				.BindInstance(_unitsConfig.Units[_species]);

			Container
				.BindInterfacesTo<UnitView>()
				.FromComponentOnRoot();

			Container
				.Bind<UnitFacade>()
				.AsSingle();

			Container
				.BindInterfacesTo<Unit>()
				.AsSingle();

			Container
				.BindInterfacesTo<UnitBuilder>()
				.AsSingle()
				.OnInstantiated<UnitBuilder>((ic, o) => o.OnInstantiated())
				.NonLazy();

			Container
				.BindInterfacesTo<UnitTargetFinder>()
				.AsSingle();

			Container
				.BindInterfacesTo<UnitMover>()
				.AsSingle();
		}
	}
}