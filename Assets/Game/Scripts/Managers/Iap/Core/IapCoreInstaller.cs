namespace Game.Iap
{
	using Zenject;

	public class IapCoreInstaller : Installer
	{
		public override void InstallBindings()
		{
			Logger.Log( Logger.Module.Iap, "-- IapCoreInstaller --" );

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
				.BindInterfacesTo< IapCoreWithValidation >()
				.FromNewComponentOnNewGameObject()
				.AsSingle();
		}
	}
}

