namespace Game.Configs
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "Region", menuName = "Configs/Region", order = (int)Config.Region)]
	public class RegionConfig : ScriptableObject
	{
		[SerializeField] private LevelConfig[] _levels;

		public LevelConfig[] Levels => _levels;
	}
}