namespace Game.Installers
{
	using Game.Configs;
	using Game.Core;
	using Game.Dev;
	using Zenject;
	using static Game.Configs.DevConfig;

	public class DevInstaller : MonoInstaller
	{
		[Inject] private DevConfig _devConfig;

		public override void InstallBindings()
		{
			if (_devConfig.Build == BuildType.Release)
				return;

			Container
				.BindInterfacesTo<DevCheats>()
				.AsSingle();

			Container
				.BindInterfacesTo<CoreTests>()
				.AsSingle();

			Container
				.Bind<DevHud>()
				.FromComponentInHierarchy()
				.AsSingle();
		}
	}
}