namespace Game.Configs
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "Levels", menuName = "Configs/Levels", order = (int)Config.Levels)]
	public class LevelsConfig : ScriptableObject
	{
		[SerializeField] private LevelConfig[] _levels;

		public LevelConfig[] Levels => _levels;
	}
}