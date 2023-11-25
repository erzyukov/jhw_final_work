namespace Game.Core
{
	using Game.Configs;
	using Game.Profiles;
	using Game.Ui;
	using Game.Utilities;
	using System;
	using UniRx;
	using UnityEngine;
	using UnityEngine.Profiling;
	using Zenject;

	public interface IGameLevel
	{
		BoolReactiveProperty IsLevelLoaded { get; }
		ReactiveCommand LevelFinished { get; }
		void GoToLevel(int number);
		void GoToNextWave();
		void FinishLevel();
		int GetLevelExperience();
		int GetLevelReward();
	}

	public class GameLevel : ControllerBase, IGameLevel, IInitializable
	{
		[Inject] private GameProfile _profile;
		[Inject] private LevelsConfig _levelsConfig;
		[Inject] private IScenesManager _scenesManager;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IUiViel _uiViel;

		// TODO: realize level experience amout calculate
		private const int ExperienceByLevel = 100;
		// TODO: realize level soft currency amout calculate
		private const int SoftCurrencyByLevel = 35;

		public void Initialize()
		{
			_scenesManager.LevelLoaded
				.Subscribe(_ => OnLevelLoaded())
				.AddTo(this);
		}

		#region IGameLevel

		public BoolReactiveProperty IsLevelLoaded { get; } = new BoolReactiveProperty();
		
		public ReactiveCommand LevelFinished { get; } = new ReactiveCommand();

		public void GoToLevel(int number)
		{
			_gameCycle.SetState(GameState.LoadingLevel);

			_uiViel.SetActive(true, () =>
			{
				if (IsLevelLoaded.Value)
				{
					_scenesManager.UnloadLevel();
					IsLevelLoaded.Value = false;
				}

				_profile.LevelNumber.Value = ClampLevelNumber(number);

				_scenesManager.LoadLevel();
			});
		}

		public void GoToNextWave()
		{
			if (IsLevelLoaded.Value == false)
				return;

			int waveCount = _levelsConfig.Levels[_profile.LevelNumber.Value].Waves.Length;
			
			if (_profile.WaveNumber.Value < waveCount)
			{
				_gameCycle.SetState(GameState.LoadingWave);
				_uiViel.SetActive(true, () =>
				{
					_profile.WaveNumber.Value++;
					_gameCycle.SetState(GameState.TacticalStage);
				});
			}
			else
			{
				_gameCycle.SetState(GameState.WinBattle);
			}
		}

		public void FinishLevel()
		{
			_uiViel.SetActive(true, () =>
			{
				LevelFinished.Execute();
				_profile.WaveNumber.Value = 0;
				_scenesManager.UnloadLevel();
				IsLevelLoaded.Value = false;
				_gameCycle.SetState(GameState.Lobby);
			});
		}

		public int GetLevelExperience()
		{
			return ExperienceByLevel;
		}

		public int GetLevelReward()
		{
			return SoftCurrencyByLevel;
		}

		#endregion

		private void LoadNextLevel()
		{
			GoToLevel(_profile.LevelNumber.Value + 1);
		}

		int ClampLevelNumber(int number) => Mathf.Clamp(number, 1, _levelsConfig.Levels.Length);

		void OnLevelLoaded()
		{
			if (_profile.WaveNumber.Value == 0)
				_profile.WaveNumber.Value++;

			_gameCycle.SetState(GameState.TacticalStage);

			_uiViel.SetActive(false, () =>
			{
				IsLevelLoaded.Value = true;
			});
		}
	}
}