namespace Game.Installers
{
	using Battle;
	using Game.Core;
	using Game.Dev;
	using UnityEngine;
	using Zenject;

    public class LevelSceneInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			WebGLDebug.Log("[Project] LevelSceneInstaller: Configure");

			//builder.Register<BattleSimulator>(Lifetime.Singleton).AsImplementedInterfaces();
			//builder.Register<TargetFinderSelector>(Lifetime.Singleton).AsImplementedInterfaces();
		}
	}
}