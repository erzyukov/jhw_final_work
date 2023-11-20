namespace Game.Field
{
	using Game.Units;
	using Game.Utilities;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public interface IField<T>
	{
		List<IUnitFacade> Units { get; }
		bool HasFreeSpace { get; }
		void InitMap(Map<T> map);
		T GetCell(Vector2Int position);
		T GetCell(IUnitFacade unit);
		bool HasUnit(IUnitFacade unit);
		void AddUnit(IUnitFacade unit);
		void AddUnit(IUnitFacade unit, T fieldCell);
		void AddUnit(IUnitFacade unit, Vector2Int position);
		void RemoveUnit(IUnitFacade unit);
		void Clear();
	}

	public class Field<T> : IField<T> where T : FieldCell
	{
		private Map<T> _map;
		private List<IUnitFacade> _units;

		public List<IUnitFacade> Units => _units;

		public bool HasFreeSpace => _map.Any(position => _map[position].HasUnit == false);

		public void InitMap(Map<T> map)
		{
			_map = map;
			_units = new List<IUnitFacade>();
		}

		public T GetCell(Vector2Int position) =>
			_map.HasPoint(position) ? _map[position] : null;

		public T GetCell(IUnitFacade unit) =>
			_map.Where(position => _map[position].Unit == unit).Select(position => _map[position]).FirstOrDefault();

		public bool HasUnit(IUnitFacade unit) =>
			_units.Where(u => u == unit).Count() != 0;

		public void AddUnit(IUnitFacade unit)
		{
			List<T> freeCells = _map.Where(position => _map[position].HasUnit == false).Select(position => _map[position]).ToList();
			T freeCell = freeCells[UnityEngine.Random.Range(0, freeCells.Count)];

			if (freeCell == null)
				return;

			AddUnit(unit, freeCell);
		}

		public void AddUnit(IUnitFacade unit, T fieldCell)
		{
			if (fieldCell.HasUnit)
				return;

			_units.Add(unit);
			fieldCell.SetUnit(unit);
		}

		public void AddUnit(IUnitFacade unit, Vector2Int position)
		{
			if(_map[position].HasUnit == false)
				AddUnit(unit, _map[position]);
		}

		public void RemoveUnit(IUnitFacade unit)
		{
			_units.Remove(unit);
			GetCell(unit)?.Clear();
		}

		public void Clear()
		{
			for (int i = 0; i < _units.Count; i++)
			{
				_units[i].DestroyView();
				GetCell(_units[i])?.Clear();
			}

			_units.Clear();
		}
	}

	public enum FieldType
	{
		Hero,
		Enemy,
	}
}
