namespace Game.Installers
{
	using Game.Configs;
	using Game.Ui;
	using System;
	using Zenject;

	public class ScreenInstaller : MonoInstaller
	{
		[Inject] private DevConfig _devConfig;

		public override void InstallBindings()
		{
			Container
				.BindInterfacesTo<ScreenNavigator>()
				.AsSingle();

			Container
				.BindInterfacesTo<UiLobbyFlow>()
				.AsSingle();

			Container
				.BindInterfacesTo<UiLobbyScreen>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<UiLobbyPresenter>()
				.AsSingle();

			Container
				.BindInterfacesTo<UiWinScreen>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<UiWin>()
				.AsSingle();

			Container
				.BindInterfacesTo<UiLoseScreen>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<UiLose>()
				.AsSingle();

			Container
				.BindInterfacesTo<UiLevelRewardScreen>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<UiLevelReward>()
				.AsSingle();

			InstallUpgrades();

			if (_devConfig.GamePatform != EGamePatform.None)
			{
				Container
					.BindInterfacesTo<UiIapShopScreen>()
					.FromComponentInHierarchy()
					.AsSingle();

				Container
					.BindInterfacesTo<UiIapShopPresenter>()
					.AsSingle();
			}
		}

		private void InstallUpgrades()
		{
			Container
				.BindInterfacesTo<UiUpgradesScreen>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<UiUpgrades>()
				.AsSingle();

			Container
				.BindInterfacesTo<UiUpgradeFlow>()
				.AsSingle();
		}
	}
}