namespace Game.Platoon
{
	using Units;
	using Utilities;
	using System.Linq;
	using System.Collections.Generic;
	using UniRx;
	using UnityEngine;

	public interface IPlatoon
	{
		ReactiveCommand<IUnit> UnitAdded { get; }
		ReactiveCommand<IUnit> UnitRemoved { get; }
		bool HasFreeSpace { get; }
		void InitMap(Map<PlatoonCell> map);
		PlatoonCell GetPlatoonCell(Vector2Int position);
		void AddUnit(IUnit unit);
		void RemoveUnit(IUnit unit);
	}

	public class Platoon : IPlatoon
	{
		private Map<PlatoonCell> _map;
		private List<IUnit> _units;

		public ReactiveCommand<IUnit> UnitAdded { get; } = new ReactiveCommand<IUnit>();
		public ReactiveCommand<IUnit> UnitRemoved { get; } = new ReactiveCommand<IUnit>();
		public bool HasFreeSpace => _map.Any(position => _map[position].HasUnit == false);

		public void InitMap(Map<PlatoonCell> map)
		{
			_map = map;
			_units = new List<IUnit>();
		}

		public PlatoonCell GetPlatoonCell(Vector2Int position) =>
			_map[position];

		public void AddUnit(IUnit unit)
		{
			_units.Add(unit);
			PlatoonCell freeCell = _map.Where(position => _map[position].HasUnit == false).Select(position => _map[position]).FirstOrDefault();

			if (freeCell == null)
				return;

			freeCell.SetUnit(unit);
			unit.Position = freeCell.Position;
			UnitAdded.Execute(unit);
		}

		public void RemoveUnit(IUnit unit)
		{
			_units.Remove(unit);
			_map[unit.Position].Clear();
			UnitRemoved.Execute(unit);
		}
	}
}