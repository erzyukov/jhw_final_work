namespace Game.Gameplay
{
	using Game.Core;
	using Game.Field;
	using Game.Utilities;
	using UniRx;
	using Zenject;

	public class TacticalStageHandler : ControllerBase, IInitializable
	{
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IFieldHeroFacade _fieldHeroFacade;
		[Inject] private IFieldEnemyFacade _fieldEnemyFacade;

		public void Initialize()
		{
			_gameCycle.State
				.Where(state => state == GameState.TacticalStage)
				.Subscribe(_ => OnTacticalStageHandler())
				.AddTo(this);

			_fieldHeroFacade.Events.UnitAdded
				.Subscribe(unit => unit.SetDraggableActive(true))
				.AddTo(this);

			_fieldHeroFacade.Events.UnitDropped
				.Subscribe(unit => unit.ResetPosition())
				.AddTo(this);
		}

		private void OnTacticalStageHandler()
		{
			_fieldHeroFacade.SetFieldRenderEnabled(true);
			_fieldHeroFacade.SetDraggableActive(true);
			_fieldHeroFacade.ResetAlives();
			_fieldEnemyFacade.SetFieldRenderEnabled(true);

			foreach (var unit in _fieldHeroFacade.Units)
				unit.Reset();
		}
	}
}