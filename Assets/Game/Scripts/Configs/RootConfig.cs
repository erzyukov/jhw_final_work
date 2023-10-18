namespace Game.Configs
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "Root", menuName = "Configs/Root", order = (int)Config.Root)]
	public class RootConfig : ScriptableObject
	{
		[SerializeField] private ScenesConfig _scenes;
		[SerializeField] private UnitsConfig _units;
		[SerializeField] private BattleFieldConfig _battleField;

		public ScenesConfig Scenes => _scenes;
		public UnitsConfig Units => _units;
		public BattleFieldConfig BattleField => _battleField;
	}
}
