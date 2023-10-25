namespace Game.Configs
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "Enemy", menuName = "Configs/Enemy", order = (int)Config.Enemy)]
	public class EnemyConfig : ScriptableObject
	{
		[SerializeField] private EnemySpawnConfig[] _enemySpawnConfigs;

		public EnemySpawnConfig GetSpawnConfig(int levelIndex) => _enemySpawnConfigs[levelIndex];
	}
}
