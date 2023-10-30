namespace Game.Core
{
	using Configs;
	using Game.Profiles;
	using UnityEngine;
	using VContainer;

	public interface IGameLevel
	{
		int LevelIndex { get; }
		void GoToRegion(int number);
		void GoToLevel(int number);
		void GoToWave(int number);
	}

	public class GameLevel : IGameLevel
	{
		[Inject] GameProfile _profile;

		#region IGameLevel

		public int LevelIndex => _profile.LevelNumber.Value - 1;

		public void GoToRegion(int number)
		{

		}

		public void GoToLevel(int number)
		{

		}

		public void GoToWave(int number)
		{

		}

		#endregion
	}
}