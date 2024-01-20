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
		ReactiveCommand LevelLoading { get; }
		ReactiveCommand<bool> LevelLoaded { get; }
		ReactiveCommand<GameLevel.LevelResult> LevelFinished { get; }
		ReactiveCommand<int> WaveStarted { get; }
		void GoToLevel(int number);
		void GoToNextWave();
		void FinishLevel();
		void LeaveBattle();
	}

	public class GameLevel : ControllerBase, IGameLevel, IInitializable
	{
		[Inject] private IGameProfileManager _gameProfileManager;
		[Inject] private GameProfile _profile;
		[Inject] private LevelsConfig _levelsConfig;
		[Inject] private IScenesManager _scenesManager;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IUiVeil _uiViel;

		private int _heroLastLevel;

		public enum LevelResult
		{
			Win,
			Fail,
			Leave
		}

		public void Initialize()
		{
			_heroLastLevel = _profile.HeroLevel.Value;

			_scenesManager.LevelLoaded
				.Subscribe(_ => OnLevelLoaded())
				.AddTo(this);

			_gameCycle.State
				.Where(state => state == GameState.TacticalStage)
				.Subscribe(_ => _uiViel.Fade())
				.AddTo(this);

			_gameCycle.State
				.Where(state => state == GameState.WinBattle)
				.Subscribe(_ => OnStateLevelWon())
				.AddTo(this);

			_gameCycle.State
				.Where(state => state == GameState.LoseBattle)
				.Subscribe(_ => OnStateLevelFailed())
				.AddTo(this);
		}

		#region IGameLevel

		public BoolReactiveProperty IsLevelLoaded { get; } = new BoolReactiveProperty();
		
		public ReactiveCommand LevelLoading { get; } = new ReactiveCommand();

		public ReactiveCommand<bool> LevelLoaded { get; } = new ReactiveCommand<bool>();

		public ReactiveCommand<LevelResult> LevelFinished { get; } = new ReactiveCommand<LevelResult>();

		public ReactiveCommand<int> WaveStarted { get; } = new ReactiveCommand<int>();

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
				Save();

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
					Save();
					_gameCycle.SetState(GameState.TacticalStage);
					WaveStarted.Execute(_profile.WaveNumber.Value);
				});
			}
			else if (_gameCycle.State.Value == GameState.CompleteWave)
			{
				_gameCycle.SetState(GameState.WinBattle);
			}
		}

		public void FinishLevel()
		{
			if (_profile.HeroLevel.Value > _heroLastLevel)
			{
				_gameCycle.SetState(GameState.HeroLevelReward);
				_heroLastLevel = _profile.HeroLevel.Value;
				
				return;
			}

			_uiViel.Appear(() =>
			{
				_profile.WaveNumber.Value = 0;
				_scenesManager.UnloadLevel();
				Save();
				IsLevelLoaded.Value = false;
				_gameCycle.SetState(GameState.Lobby);
			});
		}

		public void LeaveBattle()
		{
			LevelFinished.Execute(LevelResult.Leave);

			_uiViel.Appear(() =>
			{
				_scenesManager.UnloadLevel();
				IsLevelLoaded.Value = false;
				_gameCycle.SetState(GameState.Lobby);
			});
		}

		#endregion

		private void OnStateLevelWon()
		{
			LevelFinished.Execute(LevelResult.Win);

			_profile.LevelNumber.Value = Mathf.Clamp(_profile.LevelNumber.Value + 1, 0, _levelsConfig.Levels.Length);

			if (_profile.LevelNumber.Value <= _profile.Levels.Count)
				_profile.Levels[_profile.LevelNumber.Value - 1].Unlocked.Value = true;

			Save();
		}

		private void OnStateLevelFailed()
		{
			LevelFinished.Execute(LevelResult.Fail);
		}

		private int ClampLevelNumber(int number) => Mathf.Clamp(number, 1, _levelsConfig.Levels.Length);

		private void OnLevelLoaded()
		{
			LevelLoading.Execute();

			bool isNewLevel = _profile.WaveNumber.Value == 0;

			if (isNewLevel)
				_profile.WaveNumber.Value++;

			_gameCycle.SetState(GameState.TacticalStage);

			_uiViel.Fade(() =>
			{
				IsLevelLoaded.Value = true;
				LevelLoaded.Execute(isNewLevel);
				WaveStarted.Execute(_profile.WaveNumber.Value);
			});
		}

		private void Save() =>
			_gameProfileManager.Save();
	}
}