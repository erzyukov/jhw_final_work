using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	[CreateAssetMenu(fileName = "Scenes", menuName = "Configs/Scenes", order = (int)Config.Scenes)]
	public class ScenesConfig : ScriptableObject
	{
		public SceneField Splash;
		public SceneField Main;

		public List<SceneField> Levels;
	}
}
