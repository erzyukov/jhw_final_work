using UnityEngine;

namespace Game
{
	[CreateAssetMenu(fileName = "Root", menuName = "Configs/Root", order = (int)Config.Root)]
	public class RootConfig : ScriptableObject
	{

		// Technical
		public ScenesConfig Scenes;
	}
}
