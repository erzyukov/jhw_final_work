namespace Game.Core
{
	using Game.Profiles;
	using Game.Utilities;
	using Game.Configs;
	using Zenject;
	using DG.Tweening;
	using UniRx;
	using System.Linq;
	using UnityEngine;

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
			_profile.HeroExperience
				.Subscribe(v => Debug.LogWarning($"--------> {v}"))
				.AddTo(this);

			Debug.LogWarning($"L: {_profile.HeroLevel.Value} | {_experienceConfig.GetExperienceToLevel(_profile.HeroLevel.Value + 1)} | {_profile.HeroExperience.Value} | {_experienceConfig.GetLevelExperience(_profile.HeroLevel.Value + 1)}");

			AnimatedLevelNumber.Value = _profile.HeroLevel.Value;
			//ExperienceRatio.Value = (float)_profile.HeroExperience.Value / GetExperienceInNextLevel(_profile.HeroLevel.Value);
			ExperienceRatio.Value = (float)_profile.HeroExperience.Value / _experienceConfig.GetLevelExperience(_profile.HeroLevel.Value + 1);
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
		/*
		private int GetExperienceToLevel(int levelNumber)
		{
			return _experienceConfig.HeroLevels
				.Where((_, index) => index < levelNumber)
				.Sum(data => data.ExperienceToLevel);
		}

		//private int GetExperienceToLevel(int levelNumber) =>
		//	_experienceConfig.HeroLevels[levelNumber].ExperienceToLevel;

		private int GetExperienceInNextLevel(int levelNumber)
		{
			return _experienceConfig.HeroLevels
				.Where((_, index) => index <= levelNumber)
				.Sum(data => data.ExperienceToLevel);
		}
		*/
		private void AnimatieObtainedExperience()
		{
			Sequence sequence = DOTween.Sequence();
			int experienceAmount = _profile.LevelHeroExperience.Value;
			int animationFormValue = _profile.HeroExperience.Value;
			int nextLevelExperience = _experienceConfig.GetLevelExperience(_profile.HeroLevel.Value + 1) - _profile.HeroExperience.Value;
			int experienceInNextLevel = _experienceConfig.GetExperienceToLevel(_profile.HeroLevel.Value + 1);

			while (experienceAmount >= nextLevelExperience)
			{
				AppendExperienceAnimationStep(sequence, animationFormValue, experienceInNextLevel, Ease.Linear);

				sequence.AppendCallback(() => AnimatedLevelNumber.Value++);

				experienceAmount -= nextLevelExperience;
				animationFormValue += experienceInNextLevel - animationFormValue;
				
				_profile.HeroLevel.Value++;

				nextLevelExperience = _experienceConfig.GetLevelExperience(_profile.HeroLevel.Value + 1);
				experienceInNextLevel = _experienceConfig.GetExperienceToLevel(_profile.HeroLevel.Value + 1);
			}

			AppendExperienceAnimationStep(sequence, animationFormValue, animationFormValue + experienceAmount, Ease.OutSine);

			sequence.AppendCallback(() => _profile.HeroExperience.Value = experienceAmount);

			_gameProfileManager.Save();
		}

		private void AppendExperienceAnimationStep(Sequence sequence, int from, int to, Ease ease)
		{
			int level = _profile.HeroLevel.Value;
			int experienceInCurrentLevel = _experienceConfig.GetExperienceToLevel(level);  //GetExperienceInNextLevel(level - 1);
			int experienceToLevel = _experienceConfig.GetLevelExperience(level + 1);
			//int experienceToLevel = GetExperienceToLevel(level);
			float fromRatio = (float)(from - experienceInCurrentLevel) / experienceToLevel;
			float toRatio = (float)(to - experienceInCurrentLevel) / experienceToLevel;
			float duration = _timingsConfig.ExperienceAnimationDuration * (toRatio - fromRatio);

			sequence.Append(DOVirtual.Float(fromRatio, toRatio, duration, (value) => ExperienceRatio.Value = value).SetEase(ease));
		}
	}
}