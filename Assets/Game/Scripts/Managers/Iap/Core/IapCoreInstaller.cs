namespace Game.Iap
{
	using Zenject;

	public class IapCoreInstaller : Installer
	{
		public override void InstallBindings()
		{
			// IapCoreFacade
			Container
				.BindInterfacesTo< IapCoreFacade >()
				.AsSingle();

			// IapCoreListener
			Container
				.BindInterfacesTo< IapCoreListener >()
				.AsSingle();

			// IapCore
			Container
				.BindInterfacesTo< IapCore >()
				.FromNewComponentOnNewGameObject()
				.AsSingle();
		}
	}
}

