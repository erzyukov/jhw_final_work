namespace Game.Configs
{
	using System;
	using UnityEngine;

	[CreateAssetMenu(fileName = "Level", menuName = "Configs/Level", order = (int)Config.Level)]
	public class LevelConfig : ScriptableObject
	{
		[SerializeField] private WaveConfig[] _waves;

		public WaveConfig[] Waves => _waves;
	}
}