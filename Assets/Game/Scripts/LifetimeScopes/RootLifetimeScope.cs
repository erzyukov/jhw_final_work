namespace Game.LifetimeScope
{
	using Configs;
	using Core;
	using UnityEngine;
	using VContainer;
	using VContainer.Unity;

    public class RootLifetimeScope : LifetimeScope
    {
		[SerializeField] private RootConfig _rootConfig;
		
		protected override void Configure(IContainerBuilder builder)
        {
			RegisterConfigs(builder);

			GameObject scenesManagerGameObject = new GameObject("ScenesManager");
			DontDestroyOnLoad(scenesManagerGameObject);
			IScenesManager scenesManager = scenesManagerGameObject.AddComponent<ScenesManager>();
			builder.RegisterComponent(scenesManager).As<IScenesManager>();
		}

		private void RegisterConfigs(IContainerBuilder builder)
		{
			builder.RegisterInstance(_rootConfig.Scenes);
		}
	}
}