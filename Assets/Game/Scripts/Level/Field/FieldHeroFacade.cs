namespace Game.Field
{
	using Game.Units;
	using Game.Utilities;
	using System;
	using UniRx;
	using UnityEngine;

	public interface IFieldHeroFacade : IFieldFacade
	{
		IntReactiveProperty AliveUnitsCount { get; }

		void SetDraggableActive(bool value);
	}

	public class FieldHeroFacade : FieldFacade, IFieldHeroFacade
	{
		 DictionaryDisposable<IUnitFacade, IDisposable> _unitsDisposable = new DictionaryDisposable<IUnitFacade, IDisposable>();

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

		public override void RemoveUnit(IUnitFacade unit)
		{
			AliveUnitsCount.Value -= 1;
			base.RemoveUnit(unit);
		}

		public override void Clear()
		{
			AliveUnitsCount.Value = Units.Count;
			base.Clear();
		}

		#endregion

		void RegisterUnit(IUnitFacade unit)
		{
			IDisposable unitDeath = unit.Died
				.Subscribe(_ => UnitDiedHandler(unit))
				.AddTo(this);
			_unitsDisposable.Add(unit, unitDeath);
			AliveUnitsCount.Value += 1;
		}

		void UnitDiedHandler(IUnitFacade unit)
		{
			_unitsDisposable[unit].Dispose();
			_unitsDisposable.Remove(unit);
			AliveUnitsCount.Value -= 1;
		}
	}
}