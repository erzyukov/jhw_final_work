namespace Game.Installers
{
	using Game.Core;
	using Game.Dev;
	using Game.Input;
	using Game.Profiles;
	using Zenject;

	public class ProjectInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			WebGLDebug.Log("[Project] ProjectInstaller: Configure");

			InstallGameProfile();

			Container
				.BindInterfacesTo<ScenesManager>()
				.FromNewComponentOnNewGameObject()
				.AsSingle()
				.NonLazy();

			Container
				.BindInterfacesTo<InputHandler>()
				.AsSingle();

			InstallGameControllers();
		}

		private void InstallGameProfile()
		{
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