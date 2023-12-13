namespace Game.Configs
{
	using Game.Core;
	using System;
	using UnityEngine;

	[CreateAssetMenu(fileName = "Menu", menuName = "Configs/Menu", order = (int)Config.Menu)]
	public class MenuConfig : ScriptableObject
	{
		[SerializeField] private MenuLevelAccess[] _accessFromLevel;

		public MenuLevelAccess[] AccessFromLevel => _accessFromLevel;

		[Serializable]
		public struct MenuLevelAccess
		{
			public int LevelNumber;
			public GameState GameState;
		}
	}
}