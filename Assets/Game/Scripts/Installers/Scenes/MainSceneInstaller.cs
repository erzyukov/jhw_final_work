namespace Game.Installers
{
	using Game.Core;
	using UnityEngine;
	using Zenject;
	using Game.Ui;
	using Game.Tutorial;
	using UnityEngine.UI;
	using Game.Analytics;
	using Game.Configs;
	using Game.Managers;

	public class MainSceneInstaller : MonoInstaller
	{
		[Inject] private DevConfig _devConfig;

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
			InstallWindows();
			InstallTutorial();
			InstallAnalytics();
			InstallAds();
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

			Container
				.Bind<Button>()
				.FromComponentsInHierarchy()
				.AsSingle();

			Container
				.Bind<Toggle>()
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

		private void InstallWindows()
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

			Container
				.BindInterfacesTo<UiGameSettingsWindow>()
				.FromComponentsInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<UiGameSettings>()
				.AsSingle();
		}

		private void InstallTutorial()
		{
			Container
				.BindInterfacesTo<MainSceneTutorial>()
				.AsSingle();
		}

		private void InstallAnalytics()
		{
			Container
				.BindInterfacesTo<TutorialHierarchyAnalytics>()
				.AsSingle();

			Container
				.BindInterfacesTo<UiHierarchyAnalytics>()
				.AsSingle();

			Container
				.BindInterfacesTo<GameplayHierarchyAnalytics>()
				.AsSingle();

			Container
				.BindInterfacesTo<ResourcesHierarchyAnalytics>()
				.AsSingle();

			Container
				.BindInterfacesTo<ProgressionHierarchyAnalytics>()
				.AsSingle();

			Container
				.BindInterfacesTo<SettingsHierarchyAnalytics>()
				.AsSingle();
		}

		private void InstallAds()
		{
			switch (_devConfig.GamePatform)
			{
				case EGamePatform.YandexGames: 
					Container
						.BindInterfacesTo<YandexAdsManager>()
						.AsSingle();
					break;
			}
		}
	}
}