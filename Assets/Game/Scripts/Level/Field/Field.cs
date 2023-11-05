using Game.Platoon;
using Game.Units;
using Game.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Field
{

	public interface IField<T>
	{
		List<IUnit> Units { get; }
		bool HasFreeSpace { get; }
		void InitMap(Map<T> map);
		T GetCell(Vector2Int position);
		T GetCell(IUnit unit);
		void AddUnit(IUnit unit);
		void AddUnit(IUnit unit, T platoonCell);
		void RemoveUnit(IUnit unit);
	}

	public class Field<T>: IField<T> where T : FieldCell
	{
		private Map<T> _map;
		private List<IUnit> _units;

		public List<IUnit> Units => _units;

		public bool HasFreeSpace => _map.Any(position => _map[position].HasUnit == false);

		public void InitMap(Map<T> map)
		{
			_map = map;
			_units = new List<IUnit>();
		}

		public T GetCell(Vector2Int position) =>
			_map.HasPoint(position) ? _map[position] : null;

		public T GetCell(IUnit unit) =>
			_map.Where(position => _map[position].Unit == unit).Select(position => _map[position]).FirstOrDefault();

		public void AddUnit(IUnit unit)
		{
			T freeCell = _map.Where(position => _map[position].HasUnit == false).Select(position => _map[position]).FirstOrDefault();

			if (freeCell == null)
				return;

			AddUnit(unit, freeCell);
		}

		public void AddUnit(IUnit unit, T platoonCell)
		{
			if (platoonCell.HasUnit)
				return;

			_units.Add(unit);
			platoonCell.SetUnit(unit);
		}

		public void RemoveUnit(IUnit unit)
		{
			_units.Remove(unit);
			GetCell(unit)?.Clear();
		}
	}
}
