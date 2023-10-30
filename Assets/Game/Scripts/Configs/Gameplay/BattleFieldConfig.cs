namespace Game.Configs
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "BattleField", menuName = "Configs/BattleField", order = (int)Config.BattleField)]
	public class BattleFieldConfig : ScriptableObject
	{
		[SerializeField] private Vector2Int _teamFieldSize;
		[SerializeField] private float _fieldCellWidth;

		public Vector2Int TeamFieldSize => _teamFieldSize;
		public float FieldCellWidth => _fieldCellWidth;
	}
}
