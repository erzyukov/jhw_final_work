namespace Game.Units
{
	using UniRx;
	using UnityEngine;

	public interface IHeroUnit : IUnit
	{
		ReactiveCommand Rised { get; }
		ReactiveCommand PutDowned { get; }
		ReactiveCommand Focused { get; }
		ReactiveCommand Blured { get; }
		Transform Transform { get; }
	}

	public class HeroUnit : Unit, IHeroUnit
	{
		PointerEvents _pointerEvents;

		public HeroUnit(Kind kind, IUnitView unitView) : base(kind, unitView)
		{
			_pointerEvents = unitView.GameObject.AddComponent<PointerEvents>();
		}

		public ReactiveCommand Rised => _pointerEvents.PointerDowned;
		public ReactiveCommand PutDowned => _pointerEvents.PointerUped;
		public ReactiveCommand Focused => _pointerEvents.PointerEntered;
		public ReactiveCommand Blured => _pointerEvents.PointerExited;
		public Transform Transform => _unitView.GameObject.transform;
	}
}
