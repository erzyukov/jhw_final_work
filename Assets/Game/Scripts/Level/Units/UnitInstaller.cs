namespace Game.Units
{
	using Game.Configs;
	using Zenject;

	public class UnitInstaller : Installer<UnitInstaller>
	{
		[Inject] private Species _species;
		[Inject] private UnitsConfig _unitsConfig;

		// TODO: realize throught GameProfile
		private const int GradeIndex = 0;

		public override void InstallBindings()
		{
			Container
				.BindInstance(_species);

			UnitConfig unitConfig = _unitsConfig.Units[_species];
			Container
				.BindInstance(unitConfig);

			UnitGrade unitGrade = unitConfig.Grades[GradeIndex];
			Container
				.BindInstance(unitGrade);

			Container
				.BindInterfacesTo<UnitView>()
				.FromComponentOnRoot();

			Container
				.Bind<UnitFacade>()
				.AsSingle();

			Container
				.BindInterfacesTo<Unit>()
				.AsSingle();

			Container
				.BindInterfacesTo<UnitBuilder>()
				.AsSingle()
				.OnInstantiated<UnitBuilder>((ic, o) => o.OnInstantiated())
				.NonLazy();

			Container
				.BindInterfacesTo<UnitTargetFinder>()
				.AsSingle();

			Container
				.BindInterfacesTo<UnitMover>()
				.AsSingle();

			Container
				.BindInterfacesTo<UnitHealth>()
				.AsSingle();

			Container
				.BindInterfacesTo<UnitAttacker>()
				.AsSingle();
		}
	}
}