namespace Game.Installers
{
	using Game.Core;
	using Game.Dev;
	using UnityEngine;
	using Zenject;
	using Game.Ui;
	using Game.Level;

	public class MainSceneInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			InstallGameControllers();

			Container
				.Bind<Camera>()
				.FromComponentInHierarchy()
				.AsSingle();

			InstallUI();
			InstallHero();
			InstallDebugServicies();
		}

		private void InstallUI()
		{
			Container
				.BindInterfacesTo<UiViel>()
				.FromComponentInHierarchy()
				.AsSingle();
		}

		private void InstallHero()
		{
			Container
				.BindInterfacesTo<HeroSummonCurrency>()
				.AsSingle();
		}

		private void InstallDebugServicies()
		{
			Container
				.BindInterfacesTo<DevCheats>()
				.AsSingle();

			Container
				.BindInterfacesTo<CoreTests>()
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
		}
	}
}