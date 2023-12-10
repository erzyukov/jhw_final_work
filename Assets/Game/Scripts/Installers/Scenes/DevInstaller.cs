namespace Game.Installers
{
	using Game.Core;
	using Game.Dev;
	using Zenject;

	public class DevInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
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