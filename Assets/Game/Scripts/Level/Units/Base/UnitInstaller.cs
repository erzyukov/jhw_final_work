namespace Game.Units
{
	using Game.Configs;
	using Game.Utilities;
	using Zenject;

	public class UnitInstaller : Installer<UnitInstaller>
	{
		[Inject] private UnitData _unitData;
		[Inject] private UnitsConfig _unitsConfig;

		public override void InstallBindings()
		{
			#region Instances

			Container
				.BindInstance(_unitData);

			UnitConfig unitConfig = _unitsConfig.Units[_unitData.Species];
			Container
				.BindInstance(unitConfig);

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