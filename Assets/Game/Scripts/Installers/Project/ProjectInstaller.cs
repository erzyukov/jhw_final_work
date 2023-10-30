namespace Game.Installers
{
	using Configs;
	using Core;
	using Game.Profiles;
	using UnityEngine;
	using VContainer;
	using VContainer.Unity;

	public class ProjectInstaller : LifetimeScope
	{
		[SerializeField] private RootConfig _rootConfig;

		protected override void Configure(IContainerBuilder builder)
		{
			RegisterConfigs(builder);
			RegisterGameProfile(builder);
			RegisterSceneManager(builder);
		}

		private void RegisterConfigs(IContainerBuilder builder)
		{
			builder.RegisterInstance(_rootConfig.Scenes);
			builder.RegisterInstance(_rootConfig.Units);
			builder.RegisterInstance(_rootConfig.BattleField);
			builder.RegisterInstance(_rootConfig.Enemy);

			_rootConfig.Initialize();
		}

		private void RegisterGameProfile(IContainerBuilder builder)
		{
			builder.Register<GameProfileManager>(Lifetime.Singleton).AsImplementedInterfaces();

			builder.Register(container =>
				{
					IGameProfileManager gameProfileManager = container.Resolve<IGameProfileManager>();
					gameProfileManager.Initialize();

					return gameProfileManager.GameProfile;
				},
				Lifetime.Singleton
			);
		}

		private void RegisterSceneManager(IContainerBuilder builder)
		{
			GameObject scenesManagerGameObject = new GameObject("ScenesManager");
			DontDestroyOnLoad(scenesManagerGameObject);
			IScenesManager scenesManager = scenesManagerGameObject.AddComponent<ScenesManager>();
			builder.RegisterComponent(scenesManager).As<IScenesManager>();
		}
	}
}