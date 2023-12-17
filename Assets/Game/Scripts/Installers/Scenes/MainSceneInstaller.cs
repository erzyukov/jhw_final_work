namespace Game.Installers
{
	using Game.Core;
	using UnityEngine;
	using Zenject;
	using Game.Ui;

	public class MainSceneInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			InstallGameControllers();

			Container
				.Bind<Camera>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<GameProfileHandler>()
				.AsSingle();

			InstallUI();
			WindowsInstall();
		}

		private void InstallUI()
		{
			Container
				.BindInterfacesTo<UiVeil>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<UiMainMenuView>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<UiMainMenu>()
				.AsSingle();
		}

		private void InstallGameControllers()
		{
			Container
				.BindInterfacesTo<GameLevel>()
				.AsSingle();

			Container
				.BindInterfacesTo<GameHero>()
				.AsSingle();

			Container
				.BindInterfacesTo<GameUpgrades>()
				.AsSingle();

            Container
                .BindInterfacesTo<GameEnergy>()
                .AsSingle();
        }

        private void WindowsInstall()
		{
			Container
				.BindInterfacesTo<ContinueLevelWindow>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<ContinueLevelRequest>()
				.AsSingle();
		}

		private void InstallPools()
		{
		}
	}
}