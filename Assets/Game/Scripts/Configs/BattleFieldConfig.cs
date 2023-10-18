namespace Game.Configs
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "BattleField", menuName = "Configs/BattleField", order = (int)Config.BattleField)]
	public class BattleFieldConfig : ScriptableObject
	{
		[SerializeField] private Vector2Int _defaultPlatoonSize;

		public Vector2Int DefaultPlatoonSize => _defaultPlatoonSize;
	}
}
