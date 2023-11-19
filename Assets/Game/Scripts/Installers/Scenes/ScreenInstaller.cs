namespace Game.Installers
{
	using Game.Ui;
	using Zenject;

	public class ScreenInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container
				.Bind<IUiScreen>()
				.FromComponentsInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<ScreenNavigator>()
				.AsSingle();

			Container
				.BindInterfacesTo<UiLobbyScreen>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<UiLobby>()
				.AsSingle();
		}
	}
}