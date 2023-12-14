namespace Game.Profiles
{
	using Game.Configs;
	using Game.Utilities;
	using System.Collections.Generic;
	using System.IO;
	using UniRx;
	using UnityEngine;
	using YG;
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

		private GameProfile _gameProfile;

		public void OnInstantiated()
		{
			_gameProfile = new GameProfile();

			Observable.FromEvent(
					x => YandexGame.GetDataEvent += x,
					x => YandexGame.GetDataEvent -= x
				)
				.Subscribe(_ => OnYandexGameGetData())
				.AddTo(this);
		}

		#region IGameProfileManager

		public BoolReactiveProperty IsReady { get; } = new BoolReactiveProperty();

		public GameProfile GameProfile => _gameProfile;

		public void Save()
		{
			YandexGame.savesData.gameProfile = _gameProfile;
			YandexGame.SaveProgress();
		}

		public void Reset()
		{
			_gameProfile = new GameProfile();
			Save();
		}

		#endregion

		private void OnYandexGameGetData()
		{
			if (YandexGame.savesData.gameProfile == null)
				YandexGame.savesData.gameProfile = _gameProfile;
			else
				_gameProfile = YandexGame.savesData.gameProfile;

			AddMissing();
			IsReady.Value = true;
		}

		private void AddMissing()
		{
			AddMissingLevels();
			AddMissingUnits();

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

            foreach (var species in _unitsConfig.HeroUnits)
				if (_gameProfile.Units.Upgrades.ContainsKey(species) == false)
					_gameProfile.Units.Upgrades.Add(species, 1);
        }

#if UNITY_EDITOR
		private const string PathSavesEditor = "/YandexGame/WorkingData/Editor/SavesEditorYG.json";

		[UnityEditor.MenuItem("Game/Delete saved game")]
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