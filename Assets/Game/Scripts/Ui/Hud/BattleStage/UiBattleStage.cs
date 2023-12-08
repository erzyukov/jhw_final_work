namespace Game.Ui
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Core;
	using Game.Field;

	public class UiBattleStage : ControllerBase, IInitializable
	{
		[Inject] private IUiBattleStageHud _hud;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IFieldHeroFacade _fieldHeroFacade;
		[Inject] private IFieldEnemyFacade _fieldEnemyFacade;

		public void Initialize()
		{
			_fieldHeroFacade.AliveUnitsCount
				.Subscribe(count => _hud.SetHeroUnitsCount(count))
				.AddTo(this);

			_fieldEnemyFacade.Units.ObserveCountChanged()
				.Subscribe(count => _hud.SetEnemyUnitsCount(count))
				.AddTo(this);

			_gameCycle.State
				.Select(state => state == GameState.BattleStage)
				.Subscribe(_hud.SetActive)
				.AddTo(this);
		}
	}
}