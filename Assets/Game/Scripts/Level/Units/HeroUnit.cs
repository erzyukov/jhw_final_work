namespace Game.Units
{
	using UniRx;

	public interface IHeroUnit : IUnit
	{
		ReactiveCommand Rised { get; }
		ReactiveCommand PutDowned { get; }
		ReactiveCommand Focused { get; }
		ReactiveCommand Blured { get; }
	}

	public class HeroUnit : Unit, IHeroUnit
	{
		UnitPointerEvents _pointerEvents;

		public HeroUnit(Kind kind, IUnitView unitView) : base(kind, unitView)
		{
			_pointerEvents = unitView.GameObject.AddComponent<UnitPointerEvents>();
		}

		public ReactiveCommand Rised => _pointerEvents.PointerDowned;
		public ReactiveCommand PutDowned => _pointerEvents.PointerUped;
		public ReactiveCommand Focused => _pointerEvents.PointerEntered;
		public ReactiveCommand Blured => _pointerEvents.PointerExited;

	}
}
