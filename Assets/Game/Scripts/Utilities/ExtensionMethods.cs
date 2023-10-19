namespace Game.Utilities
{
	using UnityEngine;

	public static class ExtensionMethods
	{
		public static Vector3Int x0y(this Vector2Int v) =>
			new Vector3Int(v.x, 0, v.y);

		public static Vector3 WithX(this Vector3 v, float x)
		{
			v.x = x;

			return v;
		}

		public static Vector3 WithZ(this Vector3 v, float z)
		{
			v.z = z;

			return v;
		}
	}
}
