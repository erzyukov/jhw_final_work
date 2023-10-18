namespace Game.Configs
{
	using Utilities;
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "Scenes", menuName = "Configs/Scenes", order = (int)Config.Scenes)]
	public class ScenesConfig : ScriptableObject
	{
		public SceneField Splash;
		public SceneField Main;

		public List<SceneField> Levels;
	}
}
