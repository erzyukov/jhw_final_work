namespace Game.Installers
{
	using Game.Analytics;
	using Game.Configs;
	using Game.Core;
	using Game.Dev;
	using Game.Iap;
	using Game.Input;
	using Game.Managers;
	using Game.Profiles;
	using Game.Utilities;
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

			if (_devConfig.GamePatform == EGamePatform.YandexGames)
			{
				Container
					.BindInterfacesTo<YandexGameHandler>()
					.AsSingle();
			}

			Container
				.BindInterfacesTo<InputHandler>()
				.AsSingle();

#if UNITY_WEBGL && !UNITY_EDITOR
			Container
				.BindInterfacesTo<WebGLEvents>()
				.FromComponentInHierarchy()
				.AsSingle();
#endif

			InstallEvents();
			InstallAnalytics();
			InstallGameControllers();
			InstallIap();
		}

		private void InstallGameProfile()
		{
			switch (_devConfig.GamePatform)
			{
				case EGamePatform.None: 
					Container
						.BindInterfacesTo<FileProfileSaver>()
						.AsSingle();
					break;

				case EGamePatform.YandexGames: 
					Container
						.BindInterfacesTo<YandexProfileSaver>()
						.AsSingle();
					break;
			}

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

			Container
				.BindInterfacesTo<ResourceEvents>()
				.AsSingle();
		}

		private void InstallAnalytics()
		{
			if (_devConfig.DisableAnalytics)
				return;

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

		public void InstallIap()
		{
			if (_devConfig.GamePatform == EGamePatform.None)
				return;

			// IapDeliver
			// (!) Should be bound BEFORE IapCore, so it is ready to listen events before IapCore Init()
			Container
				.BindInterfacesTo<IapDeliver>()
				.AsSingle();

			// IapFacade
			Container
				.BindInterfacesTo<IapFacade>()
				.AsSingle();

			// IapCore
			Container
				.Bind<IIapCoreFacade>()
				.FromSubContainerResolve()
				.ByInstaller<IapCoreYandexInstaller>()
				.WithKernel()
				.AsSingle()
				// .WhenInjectedInto( typeof( IapDeliver ) )		// Not working
				;

			// https://github.com/modesttree/Zenject/issues/160
			Container.Resolve<IIapCoreFacade>();                    // Required for .WithKernel() above to work
		}
	}
}