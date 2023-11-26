namespace Game.Units
{
	using Game.Configs;
	using Game.Utilities;
	using Zenject;

	public class UnitInstaller : Installer<UnitInstaller>
	{
		[Inject] private Species _species;
		[Inject] private UnitsConfig _unitsConfig;

		// TODO: realize throught GameProfile
		private const int GradeIndex = 0;

		public override void InstallBindings()
		{
			#region Instances

			Container
				.BindInstance(_species);

			UnitConfig unitConfig = _unitsConfig.Units[_species];
			Container
				.BindInstance(unitConfig);

			Container
				.BindInstance(GradeIndex);

			UnitGrade unitGrade = unitConfig.Grades[GradeIndex];
			Container
				.BindInstance(unitGrade);

			#endregion

			#region View

			Container
				.BindInterfacesTo<UnitView>()
				.FromComponentOnRoot();

			Container
				.BindInterfacesTo<Draggable>()
				.FromComponentOnRoot();

			#endregion

			#region Base

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
				.BindInterfacesTo<UnitFsm>()
				.AsSingle();

			#endregion

			#region Components

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
			
			#endregion
		}
	}
}