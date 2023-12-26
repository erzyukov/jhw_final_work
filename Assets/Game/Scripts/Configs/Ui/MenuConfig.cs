namespace Game.Configs
{
	using Game.Core;
	using System;
	using UnityEngine;
    using System.Linq;

    [CreateAssetMenu(fileName = "Menu", menuName = "Configs/Menu", order = (int)Config.Menu)]
	public class MenuConfig : ScriptableObject
	{
		[SerializeField] private MenuLevelAccess[] _accessFromLevel;

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