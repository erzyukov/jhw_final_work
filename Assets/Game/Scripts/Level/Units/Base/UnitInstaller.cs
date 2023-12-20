namespace Game.Units
{
	using Game.Configs;
	using Game.Utilities;
	using Zenject;

	public class UnitInstaller : Installer<UnitInstaller>
	{
		[Inject] private Species _species;
		[Inject] private int _gradeIndex;
		[Inject] private UnitsConfig _unitsConfig;

		public override void InstallBindings()
		{
			#region Instances

			Container
				.BindInstance(_species);

			UnitConfig unitConfig = _unitsConfig.Units[_species];
			Container
				.BindInstance(unitConfig);

			Container
				.BindInstance(_gradeIndex);

			UnitGrade unitGrade = unitConfig.Grades[_gradeIndex];
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

            Container
                .BindInterfacesTo<UnitEvents>()
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

			InstallAttacker(unitConfig.Class);
			
			#endregion
		}

		private void InstallAttacker(Class unitClass)
		{
			switch (unitClass)
			{
				case Class.Melee:
					Container
						.BindInterfacesTo<UnitAttacker>()
						.AsSingle();
					break;

				case Class.Range:
					Container
						.BindInterfacesTo<UnitRangeAttacker>()
						.AsSingle();
					break;
			}
		}
	}
}