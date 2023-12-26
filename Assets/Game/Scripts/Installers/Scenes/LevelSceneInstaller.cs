namespace Game.Installers
{
	using Game.Configs;
	using Game.Dev;
	using Game.Field;
	using Game.Gameplay;
	using Game.Level;
	using Game.Profiles;
	using Game.Units;
	using Game.Weapon;
    using System;
    using Zenject;

    public class LevelSceneInstaller : MonoInstaller
	{
		[Inject] private LevelsConfig _levelsConfig;
		[Inject] private GameProfile _gameProfile;
		[Inject] private UnitsConfig _unitsConfig;
		[Inject] private WeaponsConfig _weaponsConfig;

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
				.BindInterfacesTo<UnitMerger>()
				.AsSingle();

			Container
				.BindInterfacesTo<BattleStageHandler>()
				.AsSingle();

			InstallWeapons();
			InstallUnits();
            InstallDev();
		}

        private void InstallWeapons()
		{
			Container.BindFactory<ProjectileData, Bullet, Bullet.Factory>()
				.FromMonoPoolableMemoryPool(x =>
					x.WithInitialSize(_weaponsConfig.BulletPoolSize)
						.FromComponentInNewPrefab(_weaponsConfig.BulletPrefab)
						.UnderTransformGroup("Bullets")
				);

			Container
				.BindInterfacesTo<ProjectileSpawner>()
				.AsSingle();
		}

		private void InstallUnits()
		{
			Container
				.BindFactory<Species, int, UnitFacade, UnitFacade.Factory>()
				.FromSubContainerResolve()
				.ByNewPrefabInstaller<UnitInstaller>(_unitsConfig.UnitPrefab);

			Container
				.BindFactory<UnityEngine.Object, IUnitModel, UnitModel.Factory>()
				.FromFactory<PrefabFactory<IUnitModel>>();
		}

        private void InstallDev()
        {
            Container
                .BindInterfacesTo<LevelDevCheats>()
                .AsSingle();
        }
    }
}