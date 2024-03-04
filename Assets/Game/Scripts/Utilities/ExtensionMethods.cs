namespace Game.Utilities
{
	using System.Collections.Generic;
	using System;
	using UnityEngine;
	using System.Linq;

	public static class ExtensionMethods
	{
		public static Vector3Int x0y(this Vector2Int v) => new Vector3Int(v.x, 0, v.y);

		public static Vector3 WithX(this Vector3 v, float x) { v.x = x; return v; }

		public static Vector3 WithZ(this Vector3 v, float z) { v.z = z; return v; }

		public static Vector3 WithY(this Vector3 v, float y) { v.y = y; return v; }

		public static Vector2 WithX(this Vector2 v, float x) { v.x = x; return v; }
		
		public static Vector2 WithY(this Vector2 v, float y) { v.y = y; return v; }

		public static Vector2Int WithX(this Vector2Int v, int x) { v.x = x; return v; }

		public static Vector3 x0y( this Vector2 v ) => new Vector3( v.x, 0, v.y );
		
		public static Vector2 xz( this Vector3 v ) => new Vector2( v.x, v.z );

		public static Vector2Int FloorToInt(this Vector2 v) => new Vector2Int( Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y) );

		public static void DestroyChildren(this Transform transform)
		{
			for (int i = transform.childCount - 1; i >= 0; i--)
				GameObject.Destroy(transform.GetChild(i).gameObject);
		}

		public static string ToString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
		{
			if (dictionary == null)
				throw new ArgumentNullException("dictionary");

			var items = from kvp in dictionary
						select kvp.Key + "=" + kvp.Value;

			return "{" + string.Join(",", items) + "}";
		}
	}
}
