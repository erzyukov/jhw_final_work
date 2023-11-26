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
		[SerializeField] private Color _heroEvenCollor;
		[SerializeField] private Color _heroOddCollor;
		[SerializeField] private Color _enemyEvenCollor;
		[SerializeField] private Color _enemyOddCollor;
		[SerializeField] private Color _selectedCollor;

		public Vector2Int TeamFieldSize => _teamFieldSize;
		public float FieldCellWidth => _fieldCellWidth;
		public FieldCellView FieldCellView => _fieldCellPrefab;
		public Color HeroEvenCollor => _heroEvenCollor;
		public Color HeroOddCollor => _heroOddCollor;
		public Color EnemyEvenCollor => _enemyEvenCollor;
		public Color EnemyOddCollor => _enemyOddCollor;
		public Color SelectedCollor => _selectedCollor;
	}
}
