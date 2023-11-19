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

	public class EnemyUnitSummoner : ControllerBase, IInitializable
	{
		[Inject] private IUnitSpawner _unitSpawner;
		[Inject] private IFieldEnemyFacade _fieldFacade;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private LevelConfig _levelConfig;
		[Inject] private GameProfile _gameProfile;

		public void Initialize()
		{
			_gameCycle.State
				.Where(state => state == GameState.TacticalStage)
				.Subscribe(_ => OnTacticalStageHandler())
				.AddTo(this);
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
		}
	}
}
