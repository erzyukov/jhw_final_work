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
		ReactiveCommand<PlatoonCell> PointerEnteredCell { get; }
		ReactiveCommand<PlatoonCell> PointerExitedCell { get; }
		bool HasFreeSpace { get; }
		void InitMap(Map<PlatoonCell> map);
		PlatoonCell GetCell(Vector2Int position);
		PlatoonCell GetCell(IUnit unit);
		void AddUnit(IUnit unit);
		void AddUnit(IUnit unit, IPlatoonCell platoonCell);
		void RemoveUnit(IUnit unit);
		void SetIgnoreUnistRaycast(bool value);
	}

	public class Platoon : ControllerBase, IPlatoon
	{
		private Map<PlatoonCell> _map;
		private List<IUnit> _units;

		public ReactiveCommand<IUnit> UnitAdded { get; } = new ReactiveCommand<IUnit>();
		public ReactiveCommand<IUnit> UnitRemoved { get; } = new ReactiveCommand<IUnit>();
		public ReactiveCommand<PlatoonCell> PointerEnteredCell { get; } = new ReactiveCommand<PlatoonCell>();
		public ReactiveCommand<PlatoonCell> PointerExitedCell { get; } = new ReactiveCommand<PlatoonCell>();
		public bool HasFreeSpace => _map.Any(position => _map[position].HasUnit == false);

		public void InitMap(Map<PlatoonCell> map)
		{
			_map = map;
			_units = new List<IUnit>();
			foreach (var position in _map)
			{
				_map[position].PointerEntred
					.Subscribe(_ => PointerEnteredCell.Execute(_map[position]))
					.AddTo(this);

				_map[position].PointerExited
					.Subscribe(_ => PointerExitedCell.Execute(_map[position]))
					.AddTo(this);
			}
		}

		public PlatoonCell GetCell(Vector2Int position) =>
			_map[position];

		public PlatoonCell GetCell(IUnit unit) =>
			_map.Where(position => _map[position].Unit == unit).Select(position => _map[position]).FirstOrDefault();

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
			UnitAdded.Execute(unit);
		}

		public void RemoveUnit(IUnit unit)
		{
			_units.Remove(unit);
			GetCell(unit)?.Clear();
			UnitRemoved.Execute(unit);
		}

		public void SetIgnoreUnistRaycast(bool value)
		{
            foreach (var unit in _units)
				(unit as IHeroUnit).SetIgnoreRaycast(value);
		}
	}
}