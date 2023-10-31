namespace Game.Configs
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "Regions", menuName = "Configs/Regions", order = (int)Config.Regions)]
	public class RegionsConfig : ScriptableObject
	{
		[SerializeField] private RegionConfig[] _regions;

		public RegionConfig[] Regions => _regions;
	}
}