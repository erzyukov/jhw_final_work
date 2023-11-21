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

		Dictionary<IUnitFacade, IDisposable> _unitSubscribes = new Dictionary<IUnitFacade, IDisposable>();

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
			IUnitFacade unit = _unitSpawner.SpawnHeroUnit(species);
			_fieldFacade.AddUnit(unit, position);

			_unitSubscribes.Add(unit, default);
			SubscribeToUnit(unit);
		}


		private void SubscribeToUnit(IUnitFacade unit)
		{
			_unitSubscribes[unit] = unit.Died
				.Subscribe(_ => UnitDiedHandler(unit));
		}

		private void UnitDiedHandler(IUnitFacade unit)
		{
			_fieldFacade.RemoveUnit(unit);
			_unitSubscribes[unit].Dispose();
			_unitSubscribes.Remove(unit);
		}
	}
}
