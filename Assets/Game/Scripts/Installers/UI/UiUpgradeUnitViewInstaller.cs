namespace Game.Installers
{
	using Game.Ui;
	using Zenject;

	public class UiUpgradeUnitViewInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container
				.BindInterfacesTo< UiUpgradeUnitView >()
				.FromComponentsInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo< UiUpgradeUnitPresenter >()
				.AsCached();
		}
	}
}