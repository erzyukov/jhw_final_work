namespace Game.Configs
{
	using Game.Field;
	using UnityEngine;

	[CreateAssetMenu(fileName = "BattleField", menuName = "Configs/BattleField", order = (int)Config.BattleField)]
	public class BattleFieldConfig : ScriptableObject
	{
		[SerializeField] private Vector2Int _teamFieldSize;
		[Header("Cell")]
		[SerializeField] private float _fieldCellWidth;
		[SerializeField] private FieldCellView _fieldCellPrefab;
		[Header("Cell / Colors")]
		[SerializeField] private Sprite _heroSprite;
		[SerializeField] private Sprite _enemySprite;
		[SerializeField] private Sprite _selectedSprite;

		public Vector2Int TeamFieldSize => _teamFieldSize;
		public float FieldCellWidth => _fieldCellWidth;
		public FieldCellView FieldCellView => _fieldCellPrefab;
		public Sprite HeroSprite => _heroSprite;
		public Sprite EnemySprite => _enemySprite;
		public Sprite SelectedSprite => _selectedSprite;
	}
}
