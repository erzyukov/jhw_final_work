namespace Game.Gameplay
{
	using Game.Core;
	using Game.Field;
	using Game.Utilities;
	using UniRx;
	using Zenject;

	public class BattleHandler : ControllerBase, IInitializable
	{
		[Inject] IGameCycle _gameCycle;
		[Inject] IGameLevel _gameLevel;
		[Inject] IFieldHeroFacade _fieldHeroFacade;
		[Inject] IFieldEnemyFacade _fieldEnemyFacade;

		public void Initialize()
		{
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
	}
}
