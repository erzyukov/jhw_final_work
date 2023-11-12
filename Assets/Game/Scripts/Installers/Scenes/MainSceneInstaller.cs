namespace Game.Installers
{
	using Core;
	using Game.Configs;
	using Game.Dev;
	using Game.Units;
	using UnityEngine;
	using Zenject;

    public class MainSceneInstaller : MonoInstaller
	{
		[Inject] private UnitsConfig _unitsConfig;

		public override void InstallBindings()
		{
			WebGLDebug.Log("[Project] MainSceneInstaller: Configure");

			Container
				.BindInterfacesTo<GameLevel>()
				.AsSingle();

			Container
				.Bind<Camera>()
				.FromComponentInHierarchy()
				.AsSingle();

			InstallUnits();
			InstallDebugServicies();
		}

		private void InstallUnits()
		{
			Container
				.BindFactory<Species, UnitFacade, UnitFacade.Factory>()
				.FromSubContainerResolve()
				.ByNewPrefabInstaller<UnitInstaller>(_unitsConfig.UnitPrefab);

			/*
			Container
				.BindFactory<Object, IUnitFacade, UnitFacade.Factory>()
				.FromFactory<UnitFactory>()
				.NonLazy();
			*/

			//Container.BindFactory<Object, UnitFacade, UnitFacade.Factory>().FromFactory<PrefabFactory<UnitFacade>>();
		}

		private void InstallDebugServicies()
		{
			Container
				.BindInterfacesTo<DevCheats>()
				.AsSingle();

			Container
				.BindInterfacesTo<CoreTests>()
				.AsSingle();
		}
	}
}