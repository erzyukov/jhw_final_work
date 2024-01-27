namespace Game.Installers
{
	using Game.Analytics;
	using Game.Configs;
	using Game.Core;
	using Game.Dev;
	using Game.Field;
	using Game.Gameplay;
	using Game.Level;
	using Game.Profiles;
	using Game.Units;
	using Game.Weapon;
    using System;
	using UnityEngine;
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

			Container
				.BindInterfacesTo<LevelEventsHandler>()
				.AsSingle();

			InstallWeapons();
			InstallUnits();
			InstallFx();
            InstallDev();
			InstallAnalytics();
		}

		private void InstallWeapons()
		{
			Container.BindFactory<ProjectileData, Bullet, Bullet.Factory>()
				.FromMonoPoolableMemoryPool(x =>
					x.WithInitialSize(_weaponsConfig.BulletPoolSize)
						.FromComponentInNewPrefab(_weaponsConfig.GetProjectile(ProjectileType.SniperBullet))
						.UnderTransformGroup(ProjectileType.SniperBullet.ToString())
				);

			Container.BindFactory<ProjectileData, Fireball, Fireball.Factory>()
				.FromMonoPoolableMemoryPool(x =>
					x.WithInitialSize(_weaponsConfig.BulletPoolSize)
						.FromComponentInNewPrefab(_weaponsConfig.GetProjectile(ProjectileType.Fireball))
						.UnderTransformGroup(ProjectileType.Fireball.ToString())
				);

			Container
				.BindInterfacesTo<ProjectileSpawner>()
				.AsSingle();
		}

		private void InstallUnits()
		{
			Container
				.BindFactory<UnitCreateData, UnitFacade, UnitFacade.Factory>()
				.FromSubContainerResolve()
				.ByNewPrefabInstaller<UnitInstaller>(_unitsConfig.UnitPrefab);

			Container
				.BindFactory<UnityEngine.Object, IUnitRenderer, UnitRenderer.Factory>()
				.FromFactory<PrefabFactory<IUnitRenderer>>();
		}

		private void InstallFx()
		{
			Container.BindFactory<Vector3, int, Color, DamageNumberFx, DamageNumberFx.Factory>()
				.FromMonoPoolableMemoryPool(x =>
					x.WithInitialSize(_unitsConfig.DamageFxPoolSize)
						.FromComponentInNewPrefab(_unitsConfig.DamageFxPrefab)
						.UnderTransformGroup("DamageFx")
				);
		}

		private void InstallDev()
        {
            Container
                .BindInterfacesTo<LevelDevCheats>()
                .AsSingle();
        }

		private void InstallAnalytics()
		{
			Container
				.BindInterfacesTo<UiAnalytics>()
				.AsSingle();
		}
	}
}