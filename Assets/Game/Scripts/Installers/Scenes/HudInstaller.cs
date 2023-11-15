namespace Game.Installers
{
	using Game.Ui;
	using Zenject;

	public class HudInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container
				.BindInterfacesTo<HudNavigator>()
				.AsSingle();

			Container
				.Bind<IUiHudPartition>()
				.FromComponentsInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<UiTacticalStageHud>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<UiTacticalStage>()
				.AsSingle();
		}
	}
}