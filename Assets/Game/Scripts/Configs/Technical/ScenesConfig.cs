namespace Game.Configs
{
	using Game.Utilities;
	using System.Collections.Generic;
	using UnityEngine;
	using System;

	[CreateAssetMenu(fileName = "Scenes", menuName = "Configs/Scenes", order = (int)Config.Scenes)]
	public class ScenesConfig : ScriptableObject
	{
		[SerializeField] private SceneField _splash;
		[SerializeField] private SceneField _main;
		[SerializeField] private List<RegionScene> _regions;

		public SceneField Splash => _splash;
		public SceneField Main => _main;
		public List<RegionScene> Regions => _regions;

		[Serializable]
		public struct RegionScene
		{
			public Region Region;
			public SceneField Scene;
		}
	}
}
