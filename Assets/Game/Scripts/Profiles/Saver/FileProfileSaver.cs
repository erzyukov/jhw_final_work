namespace Game.Profiles
{
	using System.IO;
	using UniRx;
	using Zenject;
	using Game.Utilities;
	using UnityEngine;
	using System;

	public class FileProfileSaver : ControllerBase, IProfileSaver, IInitializable
	{

		private const string SaveFileName = "save";
		private static string SaveFilePath => SavingSystem.MakePersistentFilePath( SaveFileName );


		public void Initialize()
		{
			Observable.Timer(TimeSpan.FromSeconds(1))
				.Subscribe( _ => SaveSystemReady.Execute() )
				.AddTo( this );
		}

		#region IProfileSaver

		public ReactiveCommand SaveSystemReady { get; } = new ReactiveCommand();

		public bool Load(out GameProfile data)
		{
			try
			{
				byte[] bytes = SavingSystem.LoadFile( SaveFilePath );
				data = SavingSystem.Deserialize< GameProfile >( bytes );
			}
			catch
			{
				data = null;
			}

			return data != null;
		}

		public void Save(GameProfile data)
		{
			byte[] bytes	= SavingSystem.SerializeToBytes( data );
			SavingSystem.SaveFile( SaveFilePath, bytes );
		}

		#endregion

#if UNITY_EDITOR
		[UnityEditor.MenuItem( "Game/Delete file \"save.dat\"" )]
		public static void DeleteSaveFile()
		{
			if (File.Exists( SaveFilePath ))
				File.Delete( SaveFilePath );
		}
#endif

	}
}
