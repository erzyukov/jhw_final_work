namespace Game.Configs
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "Level", menuName = "Configs/Level", order = (int)Config.Level)]
	public class LevelConfig : ScriptableObject
	{
		[SerializeField] private string _title;
		[SerializeField] private Region _region;
		[SerializeField] private WaveConfig[] _waves;

		public string Title => _title;
		public Region Region => _region;
		public WaveConfig[] Waves => _waves;
	}
}