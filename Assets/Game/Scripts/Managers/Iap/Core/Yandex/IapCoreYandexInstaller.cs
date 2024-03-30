namespace Game.Iap
{
	using Zenject;

	public class IapCoreYandexInstaller : Installer
	{
		public override void InstallBindings()
		{
			// IapCoreFacade
			Container
				.BindInterfacesTo< IapCoreFacade >()
				.AsSingle();

			// IapCoreListener
			Container
				.BindInterfacesTo< IapCoreYandexListener >()
				.AsSingle();

			// IapCore
			Container
				.BindInterfacesTo< IapYandexCore >()
				.AsSingle();
		}
	}
}

