namespace Game.Gameplay
{
	using Game.Configs;
	using Game.Core;
	using Game.Field;
	using Game.Utilities;
	using System;
	using UniRx;
	using Zenject;

	public class BattleHandler : ControllerBase, IInitializable
	{
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IGameLevel _gameLevel;
		[Inject] private IFieldHeroFacade _fieldHeroFacade;
		[Inject] private IFieldEnemyFacade _fieldEnemyFacade;
		[Inject] private TimingsConfig _timingsConfig;

		public void Initialize()
		{
			_gameCycle.State
				.Where(state => state == GameState.TacticalStage)
				.Subscribe(_ => OnTacticalStageHandler())
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
				Observable.Timer(TimeSpan.FromSeconds(_timingsConfig.WaveTransitionDelay))
					.Subscribe(_ =>
					{
						_gameCycle.SetState(GameState.CompleteWave);
						_gameLevel.GoToNextWave();
					})
					.AddTo(this);
			}
		}

		private void OnTacticalStageHandler()
		{
			_fieldHeroFacade.SetFieldRenderEnabled(true);
			_fieldEnemyFacade.SetFieldRenderEnabled(true);
			foreach (var unit in _fieldHeroFacade.Units)
				unit.Reset();
		}

		private void OnBattleStageHandler()
		{
			_fieldHeroFacade.SetFieldRenderEnabled(false);
			_fieldEnemyFacade.SetFieldRenderEnabled(false);

			foreach (var unit in _fieldHeroFacade.Units)
				unit.EnableAttack();

			foreach (var unit in _fieldEnemyFacade.Units)
				unit.EnableAttack();
		}
	}
}