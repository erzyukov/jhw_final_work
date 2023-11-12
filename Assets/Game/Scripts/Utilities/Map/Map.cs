namespace Game.Utilities
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public interface IMap<T> : IEnumerable<Vector2Int>
	{
		RectInt Rect { get; }
		Vector2Int Size { get; }
		Vector2Int Min { get; }
		Vector2Int Max { get; }
		T this[int x, int y] { get; set; }
		T this[Vector2Int p] { get; set; }

		bool HasPoint(Vector2Int position);
		bool HasPoint(int x, int y);
		bool IsElementOnCell(int x, int y, T cell);
		T Get(int x, int y);
		void Set(int x, int y, T cell);
	}

	public class Map<T> : IMap<T>
	{
		private RectInt _rect;
		private T[,] _cells;

		public Map(RectInt rect)
		{
			_rect = rect;
			_cells = new T[rect.width, rect.height];
		}

		public RectInt Rect => _rect;
		public Vector2Int Size => _rect.size;
		public Vector2Int Min => _rect.min;
		public Vector2Int Max => _rect.max;
		public T this[int x, int y]
		{
			get => Get(x, y);
			set => Set(x, y, value);
		}

		public T this[Vector2Int p]
		{
			get => Get(p.x, p.y);
			set => Set(p.x, p.y, value);
		}

		public bool HasPoint(Vector2Int position) =>
			HasPoint(position.x, position.y);

		public bool HasPoint(int x, int y) => 
			_rect.Contains(new Vector2Int(x, y));

		public bool IsElementOnCell(int x, int y, T cell) =>
			HasPoint(x, y) && cell.Equals(Get(x, y));

		public T Get(int x, int y) => 
			_cells[x - _rect.min.x, y - _rect.min.y];

		public void Set(int x, int y, T cell) => 
			_cells[x - _rect.min.x, y - _rect.min.y] = cell;

		public IEnumerator<Vector2Int> GetEnumerator() => 
			new RectIterator(_rect);

		IEnumerator IEnumerable.GetEnumerator() => 
			GetEnumerator();
	}
}