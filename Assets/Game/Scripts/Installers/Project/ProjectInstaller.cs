namespace Game.Installers
{
	using Game.Analytics;
	using Game.Configs;
	using Game.Core;
	using Game.Dev;
	using Game.Input;
	using Game.Profiles;
	using System;
	using UnityEngine;
	using Zenject;

	public class ProjectInstaller : MonoInstaller
	{
		[Inject] private DevConfig _devConfig;

		public override void InstallBindings()
		{
			Time.timeScale = _devConfig.TimeScale;

			WebGLDebug.Log("[Project] ProjectInstaller: Configure");

			InstallGameProfile();

			Container
				.BindInterfacesTo<Localizator>()
				.AsSingle();

			Container
				.BindInterfacesTo<ScenesManager>()
				.FromNewComponentOnNewGameObject()
				.AsSingle()
				.NonLazy();

			Container
				.BindInterfacesTo<InputHandler>()
				.AsSingle();

			// IApplicationPaused
			Container
				.BindInterfacesTo<WebGLEvents>()
				.FromComponentInHierarchy()
				.AsSingle();

			InstallEvents();
			InstallAnalytics();
			InstallGameControllers();
		}

		private void InstallGameProfile()
		{
			Container
				.BindInterfacesTo<FileProfileSaver>()
				.AsSingle();

			Container
				.BindInterfacesTo<GameProfileManager>()
				.AsSingle()
				.OnInstantiated<GameProfileManager>((ic, o) => o.OnInstantiated());

			Container
				.Bind<GameProfile>()
				.FromResolveGetter<IGameProfileManager>(gameProfileManager => gameProfileManager.GameProfile)
				.AsSingle()
				.MoveIntoAllSubContainers();
		}

		private void InstallEvents()
		{
			Container
				.BindInterfacesTo<GameplayEvents>()
				.AsSingle();
		}

		private void InstallAnalytics()
		{
			Container
				.BindInterfacesTo<GameAnalyticsSender>()
				.AsSingle();

			Container
				.BindInterfacesTo<TechnicalHierarchyAnalytics>()
				.AsSingle();
		}

		private void InstallGameControllers()
		{
			Container
				.BindInterfacesTo<GameCycle>()
				.AsSingle();

			Container
				.BindInterfacesTo<GameCurrency>()
				.AsSingle();
		}
	}
}