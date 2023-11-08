namespace Game.Installers
{
	using UnityEngine;
	using Zenject;

    public class PlatoonInstaller : MonoInstaller
	{
		[SerializeField] private PlatoonCellView _platoonCellPrefab;

		public override void InstallBindings()
		{
			/*
			builder.RegisterComponent(_platoonCellPrefab);

			PlatoonView platoonView = GetComponent<PlatoonView>();
			builder.RegisterComponent(platoonView).AsImplementedInterfaces();

			builder.Register<Platoon>(Lifetime.Scoped).AsImplementedInterfaces();
			builder.Register<PlatoonBuilder>(Lifetime.Scoped).AsImplementedInterfaces();
			*/
		}
	}
}