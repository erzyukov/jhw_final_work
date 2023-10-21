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
		ReactiveCommand<PlatoonCell> PointerEnteredToCell { get; }
		bool HasFreeSpace { get; }
		void InitMap(Map<PlatoonCell> map);
		PlatoonCell GetPlatoonCell(Vector2Int position);
		void AddUnit(IUnit unit);
		void AddUnit(IUnit unit, IPlatoonCell platoonCell);
		void RemoveUnit(IUnit unit);
	}

	public class Platoon : ControllerBase, IPlatoon
	{
		private Map<PlatoonCell> _map;
		private List<IUnit> _units;

		public ReactiveCommand<IUnit> UnitAdded { get; } = new ReactiveCommand<IUnit>();
		public ReactiveCommand<IUnit> UnitRemoved { get; } = new ReactiveCommand<IUnit>();
		public ReactiveCommand<PlatoonCell> PointerEnteredToCell { get; } = new ReactiveCommand<PlatoonCell>();
		public bool HasFreeSpace => _map.Any(position => _map[position].HasUnit == false);

		public void InitMap(Map<PlatoonCell> map)
		{
			_map = map;
			_units = new List<IUnit>();
			foreach (var position in _map)
			{
				_map[position].PointerEntred
					.Subscribe(_ => PointerEnteredToCell.Execute(_map[position]))
					.AddTo(this);
			}
		}

		public PlatoonCell GetPlatoonCell(Vector2Int position) =>
			_map[position];

		public void AddUnit(IUnit unit)
		{
			PlatoonCell freeCell = _map.Where(position => _map[position].HasUnit == false).Select(position => _map[position]).FirstOrDefault();

			if (freeCell == null)
				return;

			AddUnit(unit, freeCell);
		}

		public void AddUnit(IUnit unit, IPlatoonCell platoonCell)
		{
			if (platoonCell.HasUnit)
				return;

			_units.Add(unit);
			platoonCell.SetUnit(unit);
			unit.Position = platoonCell.Position;
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