namespace Game.Core
{
	using Game.Level;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Field;

	public class LevelEventsHandler : ControllerBase, IInitializable
	{
		[Inject] private IFieldHeroFacade _fieldHeroFacade;
		[Inject] private IGameplayEvents _gameplayEvents;
		[Inject] private IHeroUnitSummoner _heroUnitSummoner;

		public void Initialize()
		{
			_fieldHeroFacade.UnitsMerged
				.Subscribe(unit => _gameplayEvents.UnitsMerged.Execute(unit))
				.AddTo(this);

			_heroUnitSummoner.SummonedPaidUnit
				.Subscribe(v => _gameplayEvents.UnitSummoned.Execute(v))
				.AddTo(this);
		}
	}
}
