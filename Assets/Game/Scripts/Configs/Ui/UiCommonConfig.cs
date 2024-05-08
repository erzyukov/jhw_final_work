namespace Game.Configs
{
	using UnityEngine;

    [CreateAssetMenu(fileName = "UiCommon", menuName = "Configs/UiCommon", order = (int)Config.UiCommon)]
	public class UiCommonConfig : ScriptableObject
	{
		[Header("Materials")]
		public Material BlockedElementMaterial;
	}
}