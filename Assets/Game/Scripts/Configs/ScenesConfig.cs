namespace Game.Configs
{
	using Utilities;
	using System.Collections.Generic;
	using UnityEngine;

	[CreateAssetMenu(fileName = "Scenes", menuName = "Configs/Scenes", order = (int)Config.Scenes)]
	public class ScenesConfig : ScriptableObject
	{
		[SerializeField] private SceneField _splash;
		[SerializeField] private SceneField _main;
		[SerializeField] private List<SceneField> _levels;

		public SceneField Splash => _splash;
		public SceneField Main => _main;
		public List<SceneField> Levels => _levels;
	}
}
