namespace Game.Configs
{
	using Game.Units;
	using System;
	using UnityEngine;

	[CreateAssetMenu(fileName = "Wave", menuName = "Configs/Wave", order = (int)Config.Wave)]
	public class WaveConfig : ScriptableObject
	{
		[SerializeField] private WaveUnit[] _units;

		public WaveUnit[] Units => _units;

		[Serializable]
		public struct WaveUnit
		{
			public Vector2Int Position;
			public Species Species;
		}
	}
}