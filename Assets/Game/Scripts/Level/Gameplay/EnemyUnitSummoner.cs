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
	using System.Linq;

	public class EnemyUnitSummoner : ControllerBase, IInitializable
	{
		[Inject] private IUnitSpawner _unitSpawner;
		[Inject] private IFieldEnemyFacade _fieldFacade;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private LevelConfig _levelConfig;
		[Inject] private GameProfile _gameProfile;
		[Inject] private TimingsConfig _timingsConfig;

		Dictionary<IUnitFacade, IDisposable> _unitDiedSubscribes = new Dictionary<IUnitFacade, IDisposable>();
		Dictionary<IUnitFacade, IDisposable> _vanishSubscribes = new Dictionary<IUnitFacade, IDisposable>();

		public void Initialize()
		{
			_gameCycle.State
				.Where(state => state == GameState.LoadingWave)
				.Subscribe(_ => OnLoadingWave())
				.AddTo(this);

			_gameCycle.State
				.Where(state => state == GameState.TacticalStage)
				.Subscribe(_ => OnTacticalStage())
				.AddTo(this);

			_gameCycle.State
				.Where(state => state == GameState.CompleteWave)
				.Subscribe(_ => OnCompleteWave())
				.AddTo(this);
		}

		private void OnLoadingWave()
		{
			_fieldFacade.Clear();
		}

		private void OnTacticalStage()
		{
			var waveUnits = _levelConfig.Waves[_gameProfile.WaveNumber.Value - 1].Units;
			for (int i = 0; i < waveUnits.Length; i++)
				Summon(waveUnits[i].Species, waveUnits[i].Position, waveUnits[i].GradeIndex, waveUnits[i].Power);
		}

		private void OnCompleteWave()
		{
			var units = _vanishSubscribes.Keys.ToList();
			for(var i = 0; i < units.Count; i++)
				RemoveDeadUnit(units[i]);
		}

		private void Summon(Species species, Vector2Int position, int gradeIndex, int power)
		{
			IUnitFacade unit = _unitSpawner.SpawnEnemyUnit(species, gradeIndex, power);
			_fieldFacade.AddUnit(unit, position);

            _unitDiedSubscribes.Add(unit, default);

            SubscribeToUnit(unit);
		}


		private void SubscribeToUnit(IUnitFacade unit)
		{
            _unitDiedSubscribes[unit] = unit.Died
                .Subscribe(_ => OnUnitDied(unit));

        }

        private void OnUnitDied(IUnitFacade unit)
        {
            _unitDiedSubscribes[unit].Dispose();
            _unitDiedSubscribes.Remove(unit);

			IDisposable vanish = Observable.Timer(TimeSpan.FromSeconds(_timingsConfig.UnitDeathVanishDelay))
				.Subscribe(_ => RemoveDeadUnit(unit))
				.AddTo(this);

			_fieldFacade.RemoveUnit(unit);
			_vanishSubscribes.Add(unit, vanish);
		}

		private void RemoveDeadUnit(IUnitFacade unit)
		{
			_vanishSubscribes[unit]?.Dispose();
			_vanishSubscribes.Remove(unit);

			unit.Destroy();
		}
	}
}
