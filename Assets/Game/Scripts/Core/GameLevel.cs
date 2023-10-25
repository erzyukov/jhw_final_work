namespace Game.Core
{
	using Configs;
	using UnityEngine;
	using VContainer;

	public interface IGameLevel
	{
		int LevelIndex { get; }
		Vector2Int PlatoonSize { get; }
	}

	public class GameLevel : IGameLevel
	{
		[Inject] private BattleFieldConfig _battleFieldConfig;

		public int LevelIndex => 0;
		public Vector2Int PlatoonSize => _battleFieldConfig.DefaultPlatoonSize;
	}
}