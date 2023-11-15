namespace Game.Installers
{
	using Game.Ui;
	using UnityEngine;
	using Zenject;

	public class HudInstaller : MonoInstaller
	{
		[SerializeField] private SceneContext _sceneContext;

		public override void InstallBindings()
		{
			if (_sceneContext == SceneContext.Main)
			{
				Container
					.BindInterfacesTo<HudNavigator>()
					.AsSingle();

				Container
					.Bind<IUiHudPartition>()
					.FromComponentsInHierarchy()
					.AsSingle();
			}

			TacticalStageInstall();
			CommonGameplayInstall();
		}

		private void TacticalStageInstall()
		{
			if (_sceneContext == SceneContext.Main)
			{
				Container
					.BindInterfacesTo<UiTacticalStageHud>()
					.FromComponentInHierarchy()
					.AsSingle();
			}

			if (_sceneContext == SceneContext.Level)
			{
				Container
					.BindInterfacesTo<UiTacticalStage>()
					.AsSingle();
			}
		}

		private void CommonGameplayInstall()
		{
			if (_sceneContext == SceneContext.Main)
			{
				Container
					.BindInterfacesTo<UiCommonGameplayHud>()
					.FromComponentInHierarchy()
					.AsSingle();
			}

			if (_sceneContext == SceneContext.Level)
			{
				Container
					.BindInterfacesTo<UiCommonGameplay>()
					.AsSingle();
			}
		}

		public enum SceneContext
		{
			Main,
			Level
		}
	}
}