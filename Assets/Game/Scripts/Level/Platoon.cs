namespace Game.Level
{
	using Game.Utilities;
	using UnityEngine;

	public interface IPlatoon
	{
		void InitMap(Map<PlatoonCell> map);
	}

	public class Platoon : IPlatoon
	{
		private Map<PlatoonCell> _map;

		public void InitMap(Map<PlatoonCell> map)
		{
			_map = map;

			foreach (Vector2Int cellPosition in _map)
			{
				Debug.LogWarning(_map[cellPosition]);
			}
		}
	}
}