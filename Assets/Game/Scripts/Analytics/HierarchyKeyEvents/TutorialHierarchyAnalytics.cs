namespace Game.Analytics
{
	using Game.Profiles;
	using Game.Tutorial;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Core;
	using System;
	using Game.Ui;
	using UnityEngine;

	public class TutorialHierarchyAnalytics : HierarchyAnalyticsBase, IInitializable
	{
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IUiLevelRewardScreen _uiLevelRewardScreen;

		private const string TutorialEventKey = "Tutorial";

		private const int StepLevelReward = 15;
		private const int StepLevelRewardClosing = 16;
		private const int StepPlayHint = 17;
		private const int StepSecondBattle = 18;
		private const int StepSecondLevelComplete = 19;
		private const int StepGoToLevelThreeLobby = 20;
		private const int StepUpgradeTutorialComplete = 28;
		private const int StepThirdLevelComplete = 29;

		private int Step => GameProfile.Analytics.TutorialStep;

		private float _lastStepTime;

		public void Initialize()
		{
			if (Step == StepThirdLevelComplete)
				return;

			int beginnerTutorialStepCount = Enum.GetNames(typeof(BeginnerStep)).Length - 1;
			int upgradeTutorialStepCount = Enum.GetNames(typeof(UpgradesStep)).Length - 1;

			// Begginer Tutorial - 1-14
			if (GameProfile.Tutorial.BeginnerStep.Value != BeginnerStep.Complete)
			{
				GameProfile.Tutorial.BeginnerStep
					.Skip(1)
					.Where(v => v != BeginnerStep.None)
					.Subscribe(v => SendTutorialStep((int)v))
					.AddTo(this);
			}

			// Hero Level Reward - 15
			_gameCycle.State
				.Where(s => s == GameState.HeroLevelReward && Step == beginnerTutorialStepCount)
				.Subscribe(_ => SendTutorialStep(StepLevelReward))
				.AddTo(this);

			// Level Reward Closing - 16
			_uiLevelRewardScreen.Closed
				.Where(_ => Step == StepLevelReward)
				.Subscribe(_ => SendTutorialStep(StepLevelRewardClosing))
				.AddTo(this);

			// Play Hint Tutorial - 17
			_gameCycle.State
				.Where(s => s == GameState.Lobby && GameProfile.Tutorial.IsBattleHintComplete.Value == false)
				.Subscribe(_ => SendTutorialStep(StepPlayHint))
				.AddTo(this);

			// Second Level Start - 18
			_gameCycle.State
				.Where(s => s == GameState.LoadingLevel && Step == StepPlayHint)
				.Subscribe(_ => SendTutorialStep(StepSecondBattle))
				.AddTo(this);

			// Second Level Complete - 19
			_gameCycle.State
				.Where(s => s == GameState.WinBattle && Step == StepSecondBattle)
				.Subscribe(_ => SendTutorialStep(StepSecondLevelComplete))
				.AddTo(this);

			// Win Screen Closing - 20
			_gameCycle.State
				.Where(s => s == GameState.Lobby && Step == StepSecondLevelComplete)
				.Subscribe(_ => SendTutorialStep(StepGoToLevelThreeLobby))
				.AddTo(this);

			// Begginer Tutorial - 21-28
			if (GameProfile.Tutorial.UpgradesStep.Value != UpgradesStep.Complete)
			{
				GameProfile.Tutorial.UpgradesStep
					.Skip(1)
					.Where(v => v != UpgradesStep.None)
					.Select(v => StepGoToLevelThreeLobby + (int)v)
					.Subscribe(v => SendTutorialStep(v))
					.AddTo(this);
			}

			// Third Level Complete - 29
			_gameCycle.State
				.Where(s => s == GameState.WinBattle && Step == StepUpgradeTutorialComplete)
				.Subscribe(_ => SendTutorialStep(StepThirdLevelComplete))
				.AddTo(this);

		}

		private void SendTutorialStep(int step)
		{
			GameProfile.Analytics.TutorialStep = step;
			GameProfileManager.Save();

			//SendDesignEvent($"{TutorialEventKey}:step_{step}", Time.time - _lastStepTime);

			_lastStepTime = Time.time;
		}
	}
}