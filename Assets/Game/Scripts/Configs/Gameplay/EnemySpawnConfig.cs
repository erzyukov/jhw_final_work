namespace Game.Configs
{
	using Game.Units;
	using System;
	using UnityEngine;

	[CreateAssetMenu(fileName = "EnemySpawn", menuName = "Configs/EnemySpawn", order = (int)Config.EnemySpawn)]
	public class EnemySpawnConfig : ScriptableObject
	{
		[SerializeField] private SpawnData[] _spawnOrder;

		public SpawnData[] SpawnOrder => _spawnOrder;

		[Serializable]
		public struct SpawnData
		{
			public float Delay;
			public Vector2Int Position;
			//public Unit.Kind Type;
		}
	}
}