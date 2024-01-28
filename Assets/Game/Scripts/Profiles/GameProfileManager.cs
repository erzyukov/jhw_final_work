namespace Game.Profiles
{
	using Game.Configs;
	using Game.Units;
	using Game.Utilities;
	using System.Collections.Generic;
	using System.IO;
	using UniRx;
	using UnityEngine;
	using Zenject;

	public interface IGameProfileManager
	{
		BoolReactiveProperty IsReady { get; }
		GameProfile GameProfile { get; }
		void Save();
		void Reset();
	}

	public class GameProfileManager : ControllerBase, IGameProfileManager
	{
		[Inject] private LevelsConfig _levelsConfig;
		[Inject] private UnitsConfig _unitsConfig;
		[Inject] private EnergyConfig _energyConfig;
		[Inject] private IProfileSaver _profileSaver;

		private GameProfile _gameProfile;

		public void OnInstantiated()
		{
            CreateGameProfile();

			_profileSaver.SaveSystemReady
				.Subscribe(_ => OnSaveSystemReady())
				.AddTo(this);
		}

		#region IGameProfileManager

		public BoolReactiveProperty IsReady { get; } = new BoolReactiveProperty();

		public GameProfile GameProfile => _gameProfile;

		public void Save()
		{
			_profileSaver.Save(_gameProfile);
		}

		public void Reset()
		{
            CreateGameProfile();
			Save();
		}

		#endregion

        private void CreateGameProfile()
        {
            _gameProfile = new GameProfile();
			FillUnits();
			_gameProfile.Energy.Amount.Value = _energyConfig.MaxEnery;
        }

		private void OnSaveSystemReady()
		{
			if (_profileSaver.Load(out GameProfile loadedProfile))
				_gameProfile = loadedProfile;

			AddMissing();
			IsReady.Value = true;
		}

		private void AddMissing()
		{
			AddMissingLevels();
			AddMissingUnits();
			AddMissingEnergy();
			AddMissingAnalytics();

			Save();
		}

		private void AddMissingLevels()
		{
			if (_gameProfile.Levels == null)
				_gameProfile.Levels = new List<LevelProfile>();

			while (_gameProfile.Levels.Count < _levelsConfig.Levels.Length)
			{
				bool isUnlocked = _gameProfile.Levels.Count < _gameProfile.LevelNumber.Value;
				_gameProfile.Levels.Add(new LevelProfile());
				_gameProfile.Levels[_gameProfile.Levels.Count-1].Unlocked.Value = isUnlocked;
			}
		}

		private void AddMissingUnits()
		{
			if (_gameProfile.Units == null)
				_gameProfile.Units = new UnitsProfile();

			if (_gameProfile.Units.Upgrades == null)
				_gameProfile.Units.Upgrades = new Dictionary<Species, IntReactiveProperty>();

			FillUnits();
        }

		private void FillUnits()
		{
			foreach (var species in _unitsConfig.HeroUnits)
				if (_gameProfile.Units.Upgrades.ContainsKey(species) == false)
					_gameProfile.Units.Upgrades.Add(species, new IntReactiveProperty(1));
		}

		private void AddMissingEnergy()
		{
			if (_gameProfile.Energy == null)
				_gameProfile.Energy = new EnergyProfile();
		}

		private void AddMissingAnalytics()
		{
			if (_gameProfile.Analytics == null)
				_gameProfile.Analytics = new AnalyticsProfile();
		}

#if UNITY_EDITOR
		private const string PathSavesEditor = "/YandexGame/WorkingData/Editor/SavesEditorYG.json";

		[UnityEditor.MenuItem("Game/Delete Yandex saved game")]
		public static void DeleteSaveFile()
		{
			string path = Application.dataPath + PathSavesEditor;
			if (File.Exists(path))
				File.Delete(path);
			if (File.Exists(path + ".meta"))
				File.Delete(path + ".meta");
		}
#endif
	}
}