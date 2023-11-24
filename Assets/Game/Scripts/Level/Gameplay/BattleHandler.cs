namespace Game.Gameplay
{
	using Game.Core;
	using Game.Field;
	using Game.Utilities;
	using UniRx;
	using Zenject;

	public class BattleHandler : ControllerBase, IInitializable
	{
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IGameLevel _gameLevel;
		[Inject] private IFieldHeroFacade _fieldHeroFacade;
		[Inject] private IFieldEnemyFacade _fieldEnemyFacade;

		public void Initialize()
		{
			_gameCycle.State
				.Where(state => state == GameState.TacticalStage)
				.Subscribe(_ => RestoreUnitsOnField())
				.AddTo(this);

			_gameCycle.State
				.Where(state => state == GameState.BattleStage)
				.Subscribe(_ => OnBattleStageHandler())
				.AddTo(this);

			_fieldEnemyFacade.Units.ObserveCountChanged()
				.Subscribe(OnHeroUnitsCountChanged)
				.AddTo(this);
		}

		private void OnHeroUnitsCountChanged(int count)
		{
			if (count == 0)
			{
				_gameCycle.SetState(GameState.CompleteWave);
				_gameLevel.GoToNextWave();
			}
		}

		private void RestoreUnitsOnField()
		{
			foreach (var unit in _fieldHeroFacade.Units)
				unit.Reset();
		}

		private void OnBattleStageHandler()
		{
            foreach (var unit in _fieldHeroFacade.Units)
				unit.EnableAttack();

			foreach (var unit in _fieldEnemyFacade.Units)
				unit.EnableAttack();
		}
	}
}