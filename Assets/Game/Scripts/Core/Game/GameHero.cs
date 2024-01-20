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
		int GetExperienceToLevel { get; }
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
			ExperienceRatio.Value = (float)_profile.HeroExperience.Value / _experienceConfig.GetLevelExperience(_profile.HeroLevel.Value + 1);
		}

		#region IGameHero

		public FloatReactiveProperty ExperienceRatio { get; } = new FloatReactiveProperty();

		public IntReactiveProperty AnimatedLevelNumber { get; } = new IntReactiveProperty();

		public int GetExperienceToLevel =>
			_experienceConfig.GetLevelExperience(_profile.HeroLevel.Value + 1) - _profile.HeroExperience.Value;

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

		private void AnimatieObtainedExperience()
		{
			Sequence sequence = DOTween.Sequence();
			int experienceAmount = _profile.LevelHeroExperience.Value;
			int animationFromValue = _profile.HeroExperience.Value;
			int animationToValue = _experienceConfig.GetLevelExperience(_profile.HeroLevel.Value + 1);
			int nextLevelExperience = _experienceConfig.GetLevelExperience(_profile.HeroLevel.Value + 1) - _profile.HeroExperience.Value;
			int experienceInNextLevel = _experienceConfig.GetSummaryExperienceToLevel(_profile.HeroLevel.Value + 1);

			while (experienceAmount >= nextLevelExperience)
			{
				AppendExperienceAnimationStep(sequence, animationFromValue, animationToValue, Ease.Linear);

				sequence.AppendCallback(() => AnimatedLevelNumber.Value++);

				experienceAmount -= nextLevelExperience;
				animationFromValue = 0;
				
				_profile.HeroLevel.Value++;

				animationToValue = _experienceConfig.GetLevelExperience(_profile.HeroLevel.Value + 1);
				nextLevelExperience = animationToValue;
				experienceInNextLevel = _experienceConfig.GetSummaryExperienceToLevel(_profile.HeroLevel.Value + 1);
			}

			AppendExperienceAnimationStep(sequence, animationFromValue, animationFromValue + experienceAmount, Ease.OutSine);

			sequence.AppendCallback(() => _profile.HeroExperience.Value = experienceAmount);

			_gameProfileManager.Save();
		}

		private void AppendExperienceAnimationStep(Sequence sequence, int from, int to, Ease ease)
		{
			int level = _profile.HeroLevel.Value;
			int experienceInCurrentLevel = _experienceConfig.GetSummaryExperienceToLevel(level);
			int experienceToLevel = _experienceConfig.GetLevelExperience(level + 1);
			float fromRatio = (float)from / experienceToLevel;
			float toRatio = (float)to / experienceToLevel;
			float duration = _timingsConfig.ExperienceAnimationDuration * (toRatio - fromRatio);

			sequence.Append(DOVirtual.Float(fromRatio, toRatio, duration, (value) => ExperienceRatio.Value = value).SetEase(ease));
		}
	}
}