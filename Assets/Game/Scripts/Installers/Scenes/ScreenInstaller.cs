namespace Game.Installers
{
	using Game.Ui;
	using Zenject;

	public class ScreenInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
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

			Container
				.BindInterfacesTo<UiWinScreen>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<UiWin>()
				.AsSingle();
		}
	}
}