namespace Game.Field
{
	using Game.Units;
	using UniRx;
	using UnityEngine;

	public interface IFieldHeroFacade : IFieldFacade
	{
		IntReactiveProperty AliveUnitsCount { get; }
		void SetDraggableActive(bool value);
	}

	public class FieldHeroFacade : FieldFacade, IFieldHeroFacade
	{
		#region IFieldHeroFacade
		
		public IntReactiveProperty AliveUnitsCount { get; } = new IntReactiveProperty();

		public void SetDraggableActive(bool value)
		{
            foreach (var unit in Units)
				unit.SetDraggableActive(value);
        }

		#endregion

		#region IFieldFacade

		public override Vector2Int AddUnit(IUnitFacade unit)
		{
			RegisterUnit(unit);
			return base.AddUnit(unit);
		}

		public override Vector2Int AddUnit(IUnitFacade unit, Vector2Int position)
		{
			RegisterUnit(unit);
			return base.AddUnit(unit, position);
		}

		public override void Clear()
		{
			AliveUnitsCount.Value = Units.Count;
			base.Clear();
		}

		#endregion

		void RegisterUnit(IUnitFacade unit)
		{
			unit.Died.Subscribe(_ => AliveUnitsCount.Value -= 1).AddTo(this);
			AliveUnitsCount.Value += 1;
		}
	}
}