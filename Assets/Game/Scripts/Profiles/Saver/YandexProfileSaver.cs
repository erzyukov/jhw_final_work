﻿namespace Game.Profiles
{
	using Game.Utilities;
	using System.IO;
	using UniRx;
	using UnityEngine;
	using YG;
	using Zenject;

	public class YandexProfileSaver : ControllerBase, IProfileSaver, IInitializable
	{
		public void Initialize()
		{
			Observable.FromEvent(
					x => YandexGame.GetDataEvent += x,
					x => YandexGame.GetDataEvent -= x
				)
				.Subscribe(_ => SaveSystemReady.Execute())
				.AddTo(this);
		}

		#region IProfileSaver

		public ReactiveCommand SaveSystemReady { get; } = new ReactiveCommand();

		public bool Load(out GameProfile data)
		{
			data = YandexGame.savesData.gameProfile;

			return data != null;
		}

		public void Save(GameProfile data)
		{
			YandexGame.savesData.gameProfile = data;
			YandexGame.SaveProgress();
		}

		#endregion

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
