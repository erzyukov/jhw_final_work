namespace Game.Configs
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "BattleField", menuName = "Configs/BattleField", order = (int)Config.BattleField)]
	public class BattleFieldConfig : ScriptableObject
	{
		[SerializeField] private Vector2Int _defaultPlatoonSize;
		[SerializeField] private PlatoonView _platoonPrefab;
		[SerializeField] private PlatoonCellView _platoonCellPrefab;
		[SerializeField] private float _platoonCellWidth;

		public Vector2Int DefaultPlatoonSize => _defaultPlatoonSize;
		public PlatoonView PlatoonPrefab => _platoonPrefab;
		public PlatoonCellView PlatoonCellPrefab => _platoonCellPrefab;
		public float PlatoonCellWidth => _platoonCellWidth;

	}
}
