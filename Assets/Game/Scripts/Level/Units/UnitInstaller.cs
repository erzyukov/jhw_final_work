namespace Game.Units
{
	using Zenject;

	public class UnitInstaller : Installer<UnitInstaller>
	{
		[Inject] private Species _species;

		public override void InstallBindings()
		{
			Container
				.BindInstance(_species);

			Container
				.Bind<UnitFacade>()
				.AsSingle();

			Container
				.BindInterfacesTo<Unit>()
				.AsSingle();

			/*
			Container
				.BindInterfacesTo<UnitView>()
				.FromComponentOnRoot();

			Container
				.Bind<UnitView>()
				.FromNewComponentOnRoot()
				.AsSingle();
			*/
		}
	}
}