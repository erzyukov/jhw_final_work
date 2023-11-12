namespace Game.Core
{
	using Game.Configs;
	using Game.Profiles;
	using Game.Utilities;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public interface IGameLevel
	{
		BoolReactiveProperty IsLevelLoaded { get; }
		ReactiveCommand LevelFinished { get; }
		void GoToLevel(int number);
		void GoToNextWave();
	}

	public class GameLevel : ControllerBase, IGameLevel, IInitializable
	{
		[Inject] private GameProfile _profile;
		[Inject] private LevelsConfig _levelsConfig;
		[Inject] private IScenesManager _scenesManager;

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
			// TODO: Show veil screen

			if (IsLevelLoaded.Value)
			{
				_scenesManager.UnloadLevel();
				IsLevelLoaded.Value = false;
			}

			_profile.LevelNumber.Value = ClampLevelNumber(number);

			_scenesManager.LoadLevel();
		}

		public void GoToNextWave()
		{
			if (IsLevelLoaded.Value == false)
				return;

			int waveCount = _levelsConfig.Levels[_profile.LevelNumber.Value].Waves.Length;
			
			if (_profile.WaveNumber.Value < waveCount)
				_profile.WaveNumber.Value++;
			else
				FinishLevel();
		}

		#endregion

		void FinishLevel()
		{
			LevelFinished.Execute();
			_profile.WaveNumber.Value = 0;
			_scenesManager.UnloadLevel();
			IsLevelLoaded.Value = false;
		}

		int ClampLevelNumber(int number) => Mathf.Clamp(number, 1, _levelsConfig.Levels.Length);

		void OnLevelLoaded()
		{
			IsLevelLoaded.Value = true;

			// TODO: Hide veil screen
		}
	}
}