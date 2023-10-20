namespace Game.Units
{
	using UniRx;
	using UnityEngine;

	public interface IUnit
	{
		ReactiveCommand Rised { get; }
		ReactiveCommand PutDowned { get; }
		ReactiveCommand Focused { get; }
		ReactiveCommand Blured { get; }

		void SetViewParent(Transform tranform);
		Unit.Kind GetKind();
	}

	public class Unit : IUnit
	{
		private Kind _kind;
		private IUnitView _unitView;

		public Unit(Kind kind, IUnitView unitView)
		{
			_kind = kind;
			_unitView = unitView;
		}

		public ReactiveCommand Rised => _unitView.PointerDowned;
		public ReactiveCommand PutDowned => _unitView.PointerUped;
		public ReactiveCommand Focused => _unitView.PointerEntered;
		public ReactiveCommand Blured => _unitView.PointerExited;

		public void SetViewParent(Transform transform)
		{
			_unitView.SetParent(transform);
		}

		public Kind GetKind() => _kind;

		public enum Kind
		{
			HeavyTank,
			LightTank,
			Howitzer,
			Support,
		}
	}
}