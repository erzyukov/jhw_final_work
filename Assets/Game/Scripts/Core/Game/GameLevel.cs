namespace Game.Core
{
	using Game.Configs;
	using Game.Profiles;
	using Game.Ui;
	using Game.Utilities;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public interface IGameLevel
	{
		BoolReactiveProperty IsLevelLoaded { get; }
		ReactiveCommand LevelFinished { get; }
		ReactiveCommand LevelLoading { get; }
		void GoToLevel(int number);
		void GoToNextWave();
		void FinishLevel(bool isLevelComplete);
		int GetLevelExperience();
		int GetLevelReward();
	}

	public class GameLevel : ControllerBase, IGameLevel, IInitializable
	{
		[Inject] private IGameProfileManager _gameProfileManager;
		[Inject] private GameProfile _profile;
		[Inject] private LevelsConfig _levelsConfig;
		[Inject] private IScenesManager _scenesManager;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IUiVeil _uiViel;

		// TODO: realize level experience amout calculate
		private const int ExperienceByLevel = 100;
		// TODO: realize level soft currency amout calculate
		private const int SoftCurrencyByLevel = 35;

		public void Initialize()
		{
			_scenesManager.LevelLoaded
				.Subscribe(_ => OnLevelLoaded())
				.AddTo(this);

			_gameCycle.State
				.Where(state => state == GameState.TacticalStage)
				.Subscribe(_ => _uiViel.Fade())
				.AddTo(this);
		}

		#region IGameLevel

		public BoolReactiveProperty IsLevelLoaded { get; } = new BoolReactiveProperty();
		
		public ReactiveCommand LevelFinished { get; } = new ReactiveCommand();

		public ReactiveCommand LevelLoading { get; } = new ReactiveCommand();

		public void GoToLevel(int number)
		{
			_gameCycle.SetState(GameState.LoadingLevel);

			_uiViel.Appear(() =>
			{
				if (IsLevelLoaded.Value)
				{
					_scenesManager.UnloadLevel();
					IsLevelLoaded.Value = false;
				}

				_profile.LevelNumber.Value = ClampLevelNumber(number);
				_gameProfileManager.Save();

				_scenesManager.LoadLevel();
			});
		}

		public void GoToNextWave()
		{
			if (IsLevelLoaded.Value == false)
				return;

			int levelIndex = _profile.LevelNumber.Value - 1;
			int waveCount = _levelsConfig.Levels[levelIndex].Waves.Length;
			
			if (_profile.WaveNumber.Value < waveCount)
			{
				_gameCycle.SetState(GameState.LoadingWave);
				_uiViel.Appear(() =>
				{
					_profile.WaveNumber.Value++;
					_gameProfileManager.Save();
					_gameCycle.SetState(GameState.TacticalStage);
				});
			}
			else
			{
				_gameCycle.SetState(GameState.WinBattle);
			}
		}

		public void FinishLevel(bool isLevelComplete)
		{
			_uiViel.Appear(() =>
			{
				LevelFinished.Execute();
				if (isLevelComplete)
					_profile.LevelNumber.Value = Mathf.Clamp(_profile.LevelNumber.Value + 1, 0, _levelsConfig.Levels.Length);
				_profile.WaveNumber.Value = 0;
				_scenesManager.UnloadLevel();
				_gameProfileManager.Save();
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

		private int ClampLevelNumber(int number) => Mathf.Clamp(number, 1, _levelsConfig.Levels.Length);

		private void OnLevelLoaded()
		{
			LevelLoading.Execute();
			
			if (_profile.WaveNumber.Value == 0)
				_profile.WaveNumber.Value++;

			_gameCycle.SetState(GameState.TacticalStage);

			_uiViel.Fade(() =>
			{
				IsLevelLoaded.Value = true;
			});
		}
	}
}