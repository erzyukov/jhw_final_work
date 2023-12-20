namespace Game.Level
{
	using Game.Configs;
	using Game.Core;
	using Game.Field;
	using Game.Profiles;
	using Game.Units;
	using Game.Utilities;
	using UniRx;
	using Zenject;
	using UnityEngine;
	using System.Collections.Generic;
	using System;

	public class EnemyUnitSummoner : ControllerBase, IInitializable
	{
		[Inject] private IUnitSpawner _unitSpawner;
		[Inject] private IFieldEnemyFacade _fieldFacade;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private LevelConfig _levelConfig;
		[Inject] private GameProfile _gameProfile;

		Dictionary<IUnitFacade, IDisposable> _unitDyingSubscribes = new Dictionary<IUnitFacade, IDisposable>();
		Dictionary<IUnitFacade, IDisposable> _unitDiedSubscribes = new Dictionary<IUnitFacade, IDisposable>();

		public void Initialize()
		{
			_gameCycle.State
				.Where(state => state == GameState.LoadingWave)
				.Subscribe(_ => OnLoadingWaveHandler())
				.AddTo(this);

			_gameCycle.State
				.Where(state => state == GameState.TacticalStage)
				.Subscribe(_ => OnTacticalStageHandler())
				.AddTo(this);
		}

		private void OnLoadingWaveHandler()
		{
			_fieldFacade.Clear();
		}

		private void OnTacticalStageHandler()
		{
			var waveUnits = _levelConfig.Waves[_gameProfile.WaveNumber.Value - 1].Units;
			for (int i = 0; i < waveUnits.Length; i++)
				Summon(waveUnits[i].Species, waveUnits[i].Position);
		}

		private void Summon(Species species, Vector2Int position)
		{
			IUnitFacade unit = _unitSpawner.SpawnEnemyUnit(species);
			_fieldFacade.AddUnit(unit, position);

			_unitDyingSubscribes.Add(unit, default);
            _unitDiedSubscribes.Add(unit, default);

            SubscribeToUnit(unit);
		}


		private void SubscribeToUnit(IUnitFacade unit)
		{
			_unitDyingSubscribes[unit] = unit.Dying
				.Subscribe(_ => OnUnitDying(unit));

            _unitDiedSubscribes[unit] = unit.Died
                .Subscribe(_ => OnUnitDied(unit));

        }

        private void OnUnitDying(IUnitFacade unit)
		{
			_fieldFacade.RemoveUnit(unit);
			_unitDyingSubscribes[unit].Dispose();
			_unitDyingSubscribes.Remove(unit);
		}

        private void OnUnitDied(IUnitFacade unit)
        {
            unit.Destroy();
            _unitDiedSubscribes[unit].Dispose();
            _unitDiedSubscribes.Remove(unit);
        }
    }
}
