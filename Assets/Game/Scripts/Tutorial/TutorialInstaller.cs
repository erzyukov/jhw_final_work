namespace Game.Installers
{
	using Game.Tutorial;
	using UnityEngine;
	using Zenject;

	public class TutorialInstaller : MonoInstaller
	{
		[SerializeField] private SceneContext _sceneContext;

		public enum SceneContext
		{
			Main,
			Level
		}

		public override void InstallBindings()
		{
			if (_sceneContext == SceneContext.Main)
			{
				Container
					.BindInterfacesTo<FingerHint>()
					.FromComponentInHierarchy()
					.AsSingle();

				Container
					.BindInterfacesTo<FingerSlideHint>()
					.FromComponentInHierarchy()
					.AsSingle();

				Container
					.BindInterfacesTo<DialogHint>()
					.FromComponentInHierarchy()
					.AsSingle();

				Container
					.BindInterfacesTo<BeginerTutorialLoader>()
					.AsSingle();
			}

			if (_sceneContext == SceneContext.Level)
			{
				Container
					.BindInterfacesTo<BeginnerTutorial>()
					.AsSingle();
			}
		}
	}
}