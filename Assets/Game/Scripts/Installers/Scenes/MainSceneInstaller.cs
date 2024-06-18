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
	using System;

	public class MainSceneInstaller : MonoInstaller
	{
		[Inject] private DevConfig _devConfig;

		public override void InstallBindings()
		{
			InstallAds();

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

			InstallFactories();
			InstallUI();
			InstallWindows();
			InstallTutorial();
			InstallAnalytics();

			Container
				.Bind<IUiRewardedButton>()
				.FromComponentsInHierarchy()
				.AsSingle();
		}

		private void InstallFactories()
		{
			// UiUpgradeUnitViewFactory
			Container
				.BindFactory< UiUpgradeUnitViewFactory.Args, IUiUpgradeUnitView, UiUpgradeUnitView.Factory >()
				.FromFactory< UiUpgradeUnitViewFactory >();
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
				.BindInterfacesTo<GameHeroSquad>()
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
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<UiGameSettings>()
				.AsSingle();

			Container
				.BindInterfacesTo<UiRewardedEnergyWindow>()
				.FromComponentInHierarchy()
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
			if (_devConfig.DisableAnalytics)
				return;

			Container
				.BindInterfacesTo<TutorialAnalytics>()
				.AsSingle();

			Container
				.BindInterfacesTo<UiAnalytics>()
				.AsSingle();

			Container
				.BindInterfacesTo<GameplayAnalytics>()
				.AsSingle();

			Container
				.BindInterfacesTo<ResourcesAnalytics>()
				.AsSingle();

			Container
				.BindInterfacesTo<SettingsAnalytics>()
				.AsSingle();

			Container
				.BindInterfacesTo<MediationAnalytics>()
				.AsSingle();

			Container
				.BindInterfacesTo<CustomAnalytics>()
				.AsSingle();
		}

		private void InstallAds()
		{
			switch (_devConfig.GamePatform)
			{
				case EGamePatform.YandexGames: 

					//Container
					//	.BindInterfacesTo<YandexAdsProvider>()
					//	.AsSingle();

					//Container
					//	.BindInterfacesTo<YandexAdsManager>()
					//	.AsSingle();

					break;

				case EGamePatform.GooglePlay:

					Container
						.BindInterfacesTo<ApplovinAdsProvider>()
						.AsSingle();

					Container
						.BindInterfacesTo<ApplovinAdsManager>()
						.AsSingle();

					break;

				default:
					
					Container
						.BindInterfacesTo<DefaultAdsProvider>()
						.AsSingle();

					Container
						.BindInterfacesTo<AdsManager>()
						.AsSingle();
					
					break;
			}
		}
	}
}