namespace Game.Configs
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "CommonGameplay", menuName = "Configs/CommonGameplay", order = (int)Config.Common)]
	public class CommonGameplayConfig : ScriptableObject
	{
		public int DefaultReviveAttempts;
	}
}