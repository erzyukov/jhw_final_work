namespace Game.Utilities
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public struct RectIterator : IEnumerator<Vector2Int>
	{
		RectInt _rect;

		int _x;
		int _y;

		public RectIterator(Vector2Int size) : this(new RectInt(Vector2Int.zero, size)) { }

		public RectIterator(RectInt rect) : this()
		{
			_rect = rect;

			Reset();
		}

		public Vector2Int Current => _rect.min + new Vector2Int(_x, _y);

		object IEnumerator.Current => Current;

		public void Reset()
		{
			_x = -1;
			_y = 0;
		}

		public bool MoveNext()
		{
			if (++_x >= _rect.size.x)
			{
				_x = 0;

				if (++_y >= _rect.size.y)
					return false;
			}

			return true;
		}

		public void Dispose() { }
	}
}