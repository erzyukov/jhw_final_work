namespace Game.Configs
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "Enemy", menuName = "Configs/Enemy", order = (int)Config.Enemy)]
	public class EnemyConfig : ScriptableObject
	{
		[SerializeField] private EnemySpawnConfig[] _levelSpawnOrder;

		public EnemySpawnConfig GetSpawnOrder(int levelIndex) => _levelSpawnOrder[levelIndex];
	}
}
