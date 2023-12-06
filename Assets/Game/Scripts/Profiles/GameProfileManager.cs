namespace Game.Profiles
{
	using Game.Utilities;
	using System.IO;
	using UniRx;
	using UnityEngine;
	using YG;

	public interface IGameProfileManager
	{
		BoolReactiveProperty IsReady { get; }
		GameProfile GameProfile { get; }
		void Save();
	}

	public class GameProfileManager : ControllerBase, IGameProfileManager
	{
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
		
		#endregion

		private void OnYandexGameGetData()
		{
			if (YandexGame.savesData.gameProfile == null)
				YandexGame.savesData.gameProfile = _gameProfile;
			else
				_gameProfile = YandexGame.savesData.gameProfile;

			IsReady.Value = true;
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