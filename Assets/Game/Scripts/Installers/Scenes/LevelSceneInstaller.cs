namespace Game.Installers
{
	using Game.Configs;
	using Game.Dev;
	using Game.Field;
	using Game.Gameplay;
	using Game.Level;
	using Game.Profiles;
	using Game.Units;
	using Zenject;

    public class LevelSceneInstaller : MonoInstaller
	{
		[Inject] private LevelsConfig _levelsConfig;
		[Inject] private GameProfile _gameProfile;
		[Inject] private UnitsConfig _unitsConfig;

		public override void InstallBindings()
		{
			WebGLDebug.Log("[Project] LevelSceneInstaller: Configure");

			LevelConfig levelConfig = _levelsConfig.Levels[_gameProfile.LevelNumber.Value - 1];
			Container
				.BindInstance(levelConfig)
				.AsSingle();

			Container
				.BindInterfacesTo<UnitSpawner>()
				.AsSingle();

			Container
				.BindInterfacesTo<FieldHeroFacade>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<HeroUnitSummoner>()
				.AsSingle();

			Container
				.BindInterfacesTo<FieldEnemyFacade>()
				.FromComponentInHierarchy()
				.AsSingle();

			Container
				.BindInterfacesTo<EnemyUnitSummoner>()
				.AsSingle();

			Container
				.BindInterfacesTo<TacticalStageHandler>()
				.AsSingle();

			Container
				.BindInterfacesTo<BattleStageHandler>()
				.AsSingle();

			InstallUnits();
		}

		private void InstallUnits()
		{
			Container
				.BindFactory<Species, UnitFacade, UnitFacade.Factory>()
				.FromSubContainerResolve()
				.ByNewPrefabInstaller<UnitInstaller>(_unitsConfig.UnitPrefab);

			Container
				.BindFactory<UnityEngine.Object, IUnitModel, UnitModel.Factory>()
				.FromFactory<PrefabFactory<IUnitModel>>();
		}

	}
}