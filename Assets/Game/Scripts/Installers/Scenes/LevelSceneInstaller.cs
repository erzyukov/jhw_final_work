namespace Game.Installers
{
	using Game.Configs;
	using Game.Core;
	using Game.Core.Processors;
	using Game.Dev;
	using Game.Field;
	using Game.Fx;
	using Game.Gameplay;
	using Game.Level;
	using Game.Profiles;
	using Game.Units;
	using Game.Weapon;
	using Sirenix.Utilities;
	using UnityEngine;
	using Zenject;

	public class LevelSceneInstaller : MonoInstaller
	{
		[Inject] private LevelsConfig _levelsConfig;
		[Inject] private GameProfile _gameProfile;
		[Inject] private UnitsConfig _unitsConfig;
		[Inject] private WeaponsConfig _weaponsConfig;
		[Inject] private PrefabsConfig _prefabsConfig;

		private const int VfxPoolSize = 5;

		private Transform _allPoolsParent;

		public override void InstallBindings()
		{
			WebGLDebug.Log( "[Project] LevelSceneInstaller: Configure" );

			InstallAllPoolsParent();

			LevelConfig levelConfig = _levelsConfig.Levels[_gameProfile.LevelNumber.Value - 1];
			Container
				.BindInstance( levelConfig )
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
			InstallProcessors();
			InstallFx();
			InstallDev();
			InstallAnalytics();
		}

		void InstallAllPoolsParent()
		{
			// All Pools Parent
			_allPoolsParent = new GameObject( BindId.PoolsParent ).transform;
			_allPoolsParent.transform.SetParent( transform );

			Container.Bind<Transform>()
				.WithId( BindId.PoolsParent )
				.FromInstance( _allPoolsParent )
				.AsCached();
		}

		private void InstallWeapons()
		{
			Container.BindFactory<ProjectileArgs, Bullet, Bullet.Factory>()
				.FromMonoPoolableMemoryPool( x =>
					x.WithInitialSize( _weaponsConfig.ProjectilePoolSize )
						.FromComponentInNewPrefab( _weaponsConfig.GetProjectilePrefab( ProjectileType.SniperBullet ) )
						.UnderTransformGroup( ProjectileType.SniperBullet.ToString() )
				);

			Container.BindFactory<ProjectileArgs, Fireball, Fireball.Factory>()
				.FromMonoPoolableMemoryPool( x =>
					x.WithInitialSize( _weaponsConfig.ProjectilePoolSize )
						.FromComponentInNewPrefab( _weaponsConfig.GetProjectilePrefab( ProjectileType.Fireball ) )
						.UnderTransformGroup( ProjectileType.Fireball.ToString() )
				);

			Container.BindFactory<ProjectileArgs, GrenadeCapsule, GrenadeCapsule.Factory>()
				.FromMonoPoolableMemoryPool( x =>
					x.WithInitialSize( _weaponsConfig.ProjectilePoolSize )
						.FromComponentInNewPrefab( _weaponsConfig.GetProjectilePrefab( ProjectileType.GrenadeCapsule ) )
						.UnderTransformGroup( ProjectileType.GrenadeCapsule.ToString() )
				);

			Container.BindFactory<ProjectileArgs, Acidbolt, Acidbolt.Factory>()
				.FromMonoPoolableMemoryPool( x =>
					x.WithInitialSize( _weaponsConfig.ProjectilePoolSize )
						.FromComponentInNewPrefab( _weaponsConfig.GetProjectilePrefab( ProjectileType.Acidbolt ) )
						.UnderTransformGroup( ProjectileType.Acidbolt.ToString() )
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
				.ByNewPrefabInstaller<UnitInstaller>( _unitsConfig.UnitPrefab );

			Container
				.BindFactory<UnityEngine.Object, IUnitRenderer, UnitRenderer.Factory>()
				.FromFactory<PrefabFactory<IUnitRenderer>>();
		}

		private void InstallProcessors()
		{
			Container
				.BindInterfacesTo<FieldDamageProcessor>()
				.AsSingle();
		}

		private void InstallFx()
		{
			Container.BindFactory<Vector3, int, Color, DamageNumberFx, DamageNumberFx.Factory>()
				.FromMonoPoolableMemoryPool( x =>
					x.WithInitialSize( _unitsConfig.DamageFxPoolSize )
						.FromComponentInNewPrefab( _unitsConfig.DamageFxPrefab )
						.UnderTransform( CreatePoolParent( "DamageFx" ) )
				);

			_prefabsConfig.VfxPrefabs.ForEach( pair =>
			{
				VfxElement type = pair.Key;
				PooledParticleFx prefab = pair.Value;
				Transform parent = CreatePoolParent( type.ToString() );

				Container
					.BindMemoryPool<PooledParticleFx, PooledParticleFx.Pool>()
					.WithInitialSize( VfxPoolSize )
					.ExpandByOneAtATime()
					.WithFactoryArguments( type )
					.FromComponentInNewPrefab( prefab )
					.UnderTransform( parent );
			} );

			Container
				.BindInterfacesTo<EffectsSpawner>()
				.AsSingle();

			Container
				.BindInterfacesTo<LevelFxSpawner>()
				.AsSingle();
		}

		private Transform CreatePoolParent( string poolName )
		{
			Transform parent = new GameObject( poolName ).transform;

			parent.SetParent( _allPoolsParent );

			return parent;
		}

		private void InstallDev()
		{
			Container
				.BindInterfacesTo<LevelDevCheats>()
				.AsSingle();
		}

		private void InstallAnalytics()
		{
		}
	}
}