namespace Game.Ui
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Core;
	using Game.Profiles;
	using DG.Tweening;
	using Game.Configs;

	public class UiWin : ControllerBase, IInitializable
	{
		[Inject] private IUiWinScreen _screen;
		[Inject] private IGameHero _hero;
		[Inject] private IGameLevel _level;
		[Inject] private GameProfile _profile;
		[Inject] private TimingsConfig _timingsConfig;

		public void Initialize()
		{
			_screen.Opening
				.Subscribe(_ => OnScreenOpeningHandler())
				.AddTo(this);

			_screen.Closed
				.Subscribe(_ => OnScreenClosedHandler())
				.AddTo(this);
		}

		private void OnScreenOpeningHandler()
		{
			AnimatieObtainedExperience();
			_screen.SetLevelReward(_level.GetLevelReward());
		}

		private void OnScreenClosedHandler()
		{
			_level.FinishLevel();
		}

		private void AnimatieObtainedExperience()
		{
			Sequence sequence = DOTween.Sequence();
			_screen.SetExperienceRatio(_hero.GetExperienceRatio());
			int initialLevel = _profile.HeroLevel.Value;
			float initialExperienceRatio = _hero.GetExperienceRatio();
			int experienceAmount = _level.GetLevelExperience();
			int obtainedLevels = _hero.AddExperience(experienceAmount);

			for (int i = 0; i < obtainedLevels; i++)
			{
				float stepStartRatio = (i == 0) ? initialExperienceRatio : 0;
				float stepDuration = _timingsConfig.ExperienceAnimationDuration * (1 - stepStartRatio);
				sequence.Append(DOVirtual.Float(stepStartRatio, 1, stepDuration, (ratio) => _screen.SetExperienceRatio(ratio))
					.SetEase(Ease.Linear));

				int stepLevel = initialLevel + i + 1;
				sequence.AppendCallback(() => _screen.SetHeroLevel(stepLevel));
			}

			float startRatio = (obtainedLevels == 0) ? initialExperienceRatio : 0;
			float duration = _timingsConfig.ExperienceAnimationDuration / (1 - startRatio);
			sequence.Append(DOVirtual.Float(startRatio, _hero.GetExperienceRatio(), duration, (ratio) => _screen.SetExperienceRatio(ratio))
				.SetEase(Ease.OutSine));
		}
	}
}