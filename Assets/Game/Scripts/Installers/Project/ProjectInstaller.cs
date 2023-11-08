namespace Game.Installers
{
	using Configs;
	using Core;
	using Game.Dev;
	using Game.Input;
	using Game.Profiles;
	using UnityEngine;
	using Zenject;

	public class ProjectInstaller : MonoInstaller
	{
//		[SerializeField] private RootConfig _rootConfig;
/*
		[DllImport("__Internal")]
		private static extern void Hello();

		[DllImport("__Internal")]
		private static extern void PrintFloatArray(float[] array, int size);

		[DllImport("__Internal")]
		private static extern void HelloString(string str);

		[DllImport("__Internal")]
		private static extern void WebGLConsoleLog(string str);
*/

		public override void InstallBindings()
		{
			/*
						Hello();
						float[] a = new float[] { 1, 2, 3, 4, 5 };
						PrintFloatArray(a, a.Length);
						HelloString("string!!!!");
						WebGLConsoleLog("Console.log");
			*/
			WebGLDebug.Log("[Project] ProjectInstaller: Configure");

			RegisterGameProfile();

			Container
				.BindInterfacesTo<ScenesManager>()
				.FromNewComponentOnNewGameObject()
				.AsSingle()
				.NonLazy();

			Container
				.BindInterfacesTo<InputHandler>()
				.AsSingle();
		}

		private void RegisterGameProfile()
		{
			Container
				.BindInterfacesTo<GameProfileManager>()
				.AsSingle()
				.OnInstantiated<GameProfileManager>((ic, o) => o.OnInstantiated());

			Container
				.Bind<GameProfile>()
				.FromResolveGetter<IGameProfileManager>(gameProfileManager => gameProfileManager.GameProfile)
				.AsSingle()
				.MoveIntoAllSubContainers();
		}
	}
}