namespace Game.Installers
{
	using Game.Core;
	using Game.Configs;
	using Game.Dev;
	using Game.Units;
	using UnityEngine;
	using Zenject;
	using System;
	using Game.Ui;

	public class MainSceneInstaller : MonoInstaller
	{
		[Inject] private UnitsConfig _unitsConfig;

		public override void InstallBindings()
		{
			Container
				.BindInterfacesTo<GameLevel>()
				.AsSingle();

			Container
				.Bind<Camera>()
				.FromComponentInHierarchy()
				.AsSingle();

			InstallUI();
			InstallUnits();
			InstallDebugServicies();
		}

		private void InstallUI()
		{
			Container
				.Bind<IUiScreen>()
				.FromComponentsInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<ScreenNavigator>()
				.AsSingle();
		}

		private void InstallUnits()
		{
			Container
				.BindFactory<Species, UnitFacade, UnitFacade.Factory>()
				.FromSubContainerResolve()
				.ByNewPrefabInstaller<UnitInstaller>(_unitsConfig.UnitPrefab);
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