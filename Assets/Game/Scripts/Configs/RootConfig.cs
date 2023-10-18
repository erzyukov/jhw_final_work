namespace Game.Configs
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "Root", menuName = "Configs/Root", order = (int)Config.Root)]
	public class RootConfig : ScriptableObject
	{
		[SerializeField] private ScenesConfig _scenes;
		[SerializeField] private CombatUnitsConfig _combatUnits;

		public ScenesConfig Scenes => _scenes;
		public CombatUnitsConfig CombatUnits => _combatUnits;
	}
}
