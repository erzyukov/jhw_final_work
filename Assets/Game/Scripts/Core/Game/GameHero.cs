namespace Game.Core
{
	using Game.Profiles;
	using Game.Utilities;
	using Zenject;

	public interface IGameHero
	{
		float GetExperienceRatio();
		int AddExperience(int amount);
	}

	public class GameHero : ControllerBase, IGameHero, IInitializable
	{
		[Inject] private GameProfile _profile;

		private const int ExperienceToNextLevel = 40;

		public void Initialize()
		{
		}

		#region IGameHero

		public float GetExperienceRatio() =>
			(float)_profile.HeroLevelExperience.Value / ExperienceToNextLevel;

		public int AddExperience(int amount)
		{
			int totalExperience = _profile.HeroLevelExperience.Value + amount;

			int remainExperience = totalExperience % ExperienceToNextLevel;
			int obtainedLevels = totalExperience / ExperienceToNextLevel;

			_profile.HeroLevelExperience.Value = remainExperience;
			_profile.HeroLevel.Value += obtainedLevels;

			return obtainedLevels;
		}

		#endregion

	}
}
