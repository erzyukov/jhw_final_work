namespace Game.Field
{
	using Game.Units;
	using Game.Utilities;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UniRx;
	using UnityEngine;

	public interface IField<T>
	{
		ReactiveCollection<IUnitFacade> Units { get; }
		bool HasFreeSpace { get; }

		void InitMap(Map<T> map);
		T GetCell(Vector2Int position);
		T GetCell(IUnitFacade unit);
		bool HasUnit(IUnitFacade unit);
		Vector2Int AddUnit(IUnitFacade unit);
		Vector2Int AddUnit(IUnitFacade unit, T fieldCell);
		Vector2Int AddUnit(IUnitFacade unit, Vector2Int position);
		void RemoveUnit(IUnitFacade unit);
		void Clear();
		void SetFieldRenderEnabled(bool value);
		List<IFieldCell> FindSameUnitCells(IUnitFacade compareUnit);
	}

	public class Field<T> : IField<T> where T : FieldCell
	{
		private Map<T> _map;
		private ReactiveCollection<IUnitFacade> _units = new ReactiveCollection<IUnitFacade>();

		#region MyRegion

		public ReactiveCollection<IUnitFacade> Units => _units;

		public bool HasFreeSpace => _map.Any(position => _map[position].HasUnit == false);

		public void InitMap(Map<T> map)
		{
			_map = map;
		}

		public T GetCell(Vector2Int position) =>
			_map.HasPoint(position) ? _map[position] : null;

		public T GetCell(IUnitFacade unit) =>
			_map.Where(position => _map[position].Unit == unit).Select(position => _map[position]).FirstOrDefault();

		public bool HasUnit(IUnitFacade unit) =>
			_units.Where(u => u == unit).Count() != 0;

		public Vector2Int AddUnit(IUnitFacade unit)
		{
			List<T> freeCells = _map.Where(position => _map[position].HasUnit == false).Select(position => _map[position]).ToList();
			T freeCell = freeCells[UnityEngine.Random.Range(0, freeCells.Count)];

			if (freeCell == null)
				throw new Exception($"All fields are buisy!");

			return AddUnit(unit, freeCell);
		}

		public Vector2Int AddUnit(IUnitFacade unit, T fieldCell)
		{
			if (fieldCell.HasUnit)
				throw new Exception($"Field with position {fieldCell.Position} is buisy!");

			fieldCell.SetUnit(unit);
			_units.Add(unit);

			return fieldCell.Position;
		}

		public Vector2Int AddUnit(IUnitFacade unit, Vector2Int position)
		{
			return AddUnit(unit, _map[position]);
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
				GetCell(_units[i])?.Clear();
				_units[i].Destroy();
			}

			_units.Clear();
		}

		public void SetFieldRenderEnabled(bool value)
		{
            foreach (var cellPosition in _map)
				_map[cellPosition].SetFieldCellRenderEnabled(value);
        }

		public List<IFieldCell> FindSameUnitCells(IUnitFacade compareUnit)
		{
			return Units
				.Where(unit => unit.Species == compareUnit.Species && unit.GradeIndex == compareUnit.GradeIndex && unit != compareUnit)
				.Select(unit => GetCell(unit))
				.ToList<IFieldCell>();
		}

		#endregion
	}

	public enum FieldType
	{
		Hero,
		Enemy,
	}
}
