namespace Game.Configs
{
	using Game.Core;
	using System;
	using UnityEngine;
    using System.Linq;

    [CreateAssetMenu(fileName = "Menu", menuName = "Configs/Menu", order = (int)Config.Menu)]
	public class MenuConfig : ScriptableObject
	{
		[Header("Lobby")]
		[SerializeField] private MenuLevelAccess[] _accessFromLevel;
		
		[Header("Tactical")]
		[SerializeField] private int _tacticalMenuActiveFromLevel;

		public int TacticalMenuActiveFromLevel => _tacticalMenuActiveFromLevel;

		public int GetAccessLevel(GameState state) => _accessFromLevel
                .Where(p => p.GameState == state)
                .Select(p => p.LevelNumber)
                .FirstOrDefault();

        [Serializable]
		public struct MenuLevelAccess
		{
			public int LevelNumber;
			public GameState GameState;
		}
	}
}