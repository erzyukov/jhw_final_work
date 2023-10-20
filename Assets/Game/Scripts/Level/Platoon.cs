namespace Game.Level
{
	using Units;
	using Utilities;
	using UnityEngine;
	using System.Linq;
	using System.Collections.Generic;

	public interface IPlatoon
	{
		bool HasFreeSpace { get; }
		void InitMap(Map<PlatoonCell> map);
		void AddUnit(Unit unit);
	}

	public class Platoon : IPlatoon
	{
		private Map<PlatoonCell> _map;
		private List<Unit> _units;

		public bool HasFreeSpace => _map.Any(position => _map[position].HasUnit == false);

		public void InitMap(Map<PlatoonCell> map)
		{
			_map = map;
			_units = new List<Unit>();
		}

		public void AddUnit(Unit unit)
		{
			_units.Add(unit);
			PlatoonCell freeCell = _map.Where(position => _map[position].HasUnit == false).Select(position => _map[position]).FirstOrDefault();

			if (freeCell == null)
				return;

			freeCell.SetUnit(unit);
		}
	}
}