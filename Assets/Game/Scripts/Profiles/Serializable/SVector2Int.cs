namespace Game.Profiles
{
	using System;
	using UnityEngine;

	[Serializable]
	public struct SVector2Int
	{
		public int x;
		public int y;

		public SVector2Int(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static implicit operator Vector2Int(SVector2Int v) => new Vector2Int(v.x, v.y);
		public static implicit operator SVector2Int(Vector2Int v) => new SVector2Int(v.x, v.y);
	}
}