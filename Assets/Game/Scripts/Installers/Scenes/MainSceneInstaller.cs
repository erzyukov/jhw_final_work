namespace Game.Installers
{
	using Core;
	using Game.Configs;
	using Game.Dev;
	using System;
	using UnityEngine;
	using Zenject;

    public class MainSceneInstaller : MonoInstaller
	{
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

			InstallDebugServicies();

			/*
			//builder.RegisterComponentInHierarchy<HudUnitPanel>().AsImplementedInterfaces();

			//builder.Register<UnitViewFactory>(Lifetime.Singleton);
			*/
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