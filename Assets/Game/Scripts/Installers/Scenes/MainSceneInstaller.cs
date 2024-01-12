namespace Game.Installers
{
	using Game.Core;
	using UnityEngine;
	using Zenject;
	using Game.Ui;
	using Game.Tutorial;
	using System;

	public class MainSceneInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container
				.BindInterfacesTo<AudioSources>()
				.FromComponentInHierarchy()
				.AsSingle();

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
			TutorialInstall();
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

			Container
				.Bind<HintedButton>()
				.FromComponentsInHierarchy()
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

			Container
				.BindInterfacesTo<GameAudio>()
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

			Container
				.BindInterfacesTo<UiBattleMenuWindow>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<UiBattleMenu>()
				.AsSingle();
		}

		private void TutorialInstall()
		{
			Container
				.BindInterfacesTo<MainSceneTutorial>()
				.AsSingle();
		}

		private void InstallPools()
		{
		}
	}
}