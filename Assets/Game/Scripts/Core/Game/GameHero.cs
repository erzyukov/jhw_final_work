namespace Game.Core
{
	using Game.Profiles;
	using Game.Utilities;
	using Game.Configs;
	using Zenject;
	using DG.Tweening;
	using UniRx;
	using System.Linq;

	public interface IGameHero
	{
		IntReactiveProperty AnimatedLevelNumber { get; }
		FloatReactiveProperty ExperienceRatio { get; }
		void AddLevelHeroExperience(int value);
		void ResetLevelHeroExperience();
		void ConsumeLevelHeroExperience();
	}

	public class GameHero : ControllerBase, IGameHero, IInitializable
	{
		[Inject] private GameProfile _profile;
		[Inject] private IGameProfileManager _gameProfileManager;
		[Inject] private TimingsConfig _timingsConfig;
		[Inject] private ExperienceConfig _experienceConfig;

		public void Initialize()
		{
			AnimatedLevelNumber.Value = _profile.HeroLevel.Value;
			ExperienceRatio.Value = (float)_profile.HeroExperience.Value / GetExperienceInNextLevel(_profile.HeroLevel.Value);
		}

		#region IGameHero

		public FloatReactiveProperty ExperienceRatio { get; } = new FloatReactiveProperty();

		public IntReactiveProperty AnimatedLevelNumber { get; } = new IntReactiveProperty();

		public void AddLevelHeroExperience(int value) =>
			_profile.LevelHeroExperience.Value += value;

		public void ResetLevelHeroExperience() =>
			_profile.LevelHeroExperience.Value = 0;

		public void ConsumeLevelHeroExperience()
		{
			AnimatieObtainedExperience();
			ResetLevelHeroExperience();
		}

		#endregion

		private int GetExperienceToLevel(int levelNumber) =>
			_experienceConfig.HeroLevels[levelNumber].ExperienceToLevel;

		private int GetExperienceInNextLevel(int levelNumber)
		{
			return _experienceConfig.HeroLevels
				.Where((_, index) => index <= levelNumber)
				.Sum(data => data.ExperienceToLevel);
		}

		private void AnimatieObtainedExperience()
		{
			Sequence sequence = DOTween.Sequence();
			int experienceAmount = _profile.LevelHeroExperience.Value;
			int animationFormValue = _profile.HeroExperience.Value;
			int experienceToLevel = GetExperienceToLevel(_profile.HeroLevel.Value);
			int experienceInNextLevel = GetExperienceInNextLevel(_profile.HeroLevel.Value);

			while (experienceAmount >= experienceToLevel)
			{
				AppendExperienceAnimationStep(sequence, animationFormValue, experienceInNextLevel, Ease.Linear);

				sequence.AppendCallback(() => AnimatedLevelNumber.Value++);

				experienceAmount -= experienceToLevel;
				animationFormValue += experienceInNextLevel - animationFormValue;
				
				_profile.HeroLevel.Value++;

				experienceToLevel = GetExperienceToLevel(_profile.HeroLevel.Value);
				experienceInNextLevel = GetExperienceInNextLevel(_profile.HeroLevel.Value);
			}

			AppendExperienceAnimationStep(sequence, animationFormValue, animationFormValue + experienceAmount, Ease.OutSine);

			sequence.AppendCallback(() => _profile.HeroExperience.Value += experienceAmount);

			_gameProfileManager.Save();
		}

		private void AppendExperienceAnimationStep(Sequence sequence, int from, int to, Ease ease)
		{
			int level = _profile.HeroLevel.Value;
			int experienceInPreviousLevel = GetExperienceInNextLevel(level - 1);
			int experienceToLevel = GetExperienceToLevel(level);
			float fromRatio = (float)(from - experienceInPreviousLevel) / experienceToLevel;
			float toRatio = (float)(to - experienceInPreviousLevel) / experienceToLevel;
			float duration = _timingsConfig.ExperienceAnimationDuration * (toRatio - fromRatio);

			sequence.Append(DOVirtual.Float(fromRatio, toRatio, duration, (value) => ExperienceRatio.Value = value).SetEase(ease));
		}
	}
}