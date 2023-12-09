namespace Game.Core
{
	using Game.Profiles;
	using Game.Utilities;
	using Zenject;

	public interface IGameHero
	{
		float GetExperienceRatio();
		int AddExperience(int amount);
		void AddLevelHeroExperience(int value);
		void ResetLevelHeroExperience();
		void ConsumeLevelHeroExperience();
	}

	public class GameHero : ControllerBase, IGameHero, IInitializable
	{
		[Inject] private GameProfile _profile;
		[Inject] private IGameProfileManager _gameProfileManager;

		private const int ExperienceToNextLevel = 40;

		public void Initialize()
		{
		}

		#region IGameHero

		public float GetExperienceRatio() =>
			(float)_profile.HeroExperience.Value / ExperienceToNextLevel;

		public void AddLevelHeroExperience(int value) => 
			_profile.LevelHeroExperience.Value =+ value;

		public void ResetLevelHeroExperience() =>
			_profile.LevelHeroExperience.Value = 0;

		public void ConsumeLevelHeroExperience()
		{
			AddExperience(_profile.LevelHeroExperience.Value);
			ResetLevelHeroExperience();
		}

		public int AddExperience(int amount)
		{
			int totalExperience = _profile.HeroExperience.Value + amount;

			int remainExperience = totalExperience % ExperienceToNextLevel;
			int obtainedLevels = totalExperience / ExperienceToNextLevel;

			_profile.HeroExperience.Value = remainExperience;
			_profile.HeroLevel.Value += obtainedLevels;

			_gameProfileManager.Save();

			return obtainedLevels;
		}

		#endregion

	}
}