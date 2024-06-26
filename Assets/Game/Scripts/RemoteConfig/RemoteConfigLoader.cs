namespace Game.RemoteConfig
{
	using System;
	using System.Linq;
	using Zenject;
	using UniRx;
	using Game.Configs;
	using Firebase.Extensions;
	using Firebase.RemoteConfig;
	using System.Collections.Generic;
	using Newtonsoft.Json;
	using System.Threading.Tasks;

	public enum ERemoteConfigState
	{
		None,           // Wait
		TimeOut,
		Received,
	}

	public interface IRemoteConfigLoader
	{
		ReadOnlyReactiveProperty<ERemoteConfigState> State { get; }
	}

	public class RemoteConfigLoader : IRemoteConfigLoader, IInitializable
	{
#region External

		[Inject] TimingsConfig  _timingsConfig;

#endregion

		ReactiveCommand _initializeComplete = new();

		public void Initialize()
		{
			var loadDelay = _timingsConfig.MaxWaitLoadingRemoteConfig;
#if UNITY_EDITOR
			loadDelay = 1;
#endif
			State = Observable
				.Merge(
					// TimeOut
					Observable.Timer(TimeSpan.FromSeconds(loadDelay))
						.Select(_ => ERemoteConfigState.TimeOut),
					// Received
					_initializeComplete
						.Select(_ => ERemoteConfigState.Received)
				)
				.First()
				.ToReadOnlyReactiveProperty();

			Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread( task =>
			{
				var dependencyStatus = task.Result;
				if (dependencyStatus == Firebase.DependencyStatus.Available)
				{
					// Create and hold a reference to your FirebaseApp,
					// where app is a Firebase.FirebaseApp property of your application class.
					// app = Firebase.FirebaseApp.DefaultInstance;

					StartFirebaseInitialize();
				}
				else
				{
					Logger.Log( Logger.Module.RemoteConfig, $"Could not resolve all Firebase dependencies: {dependencyStatus}" );
					// Firebase Unity SDK is not safe to use here.
				}
			});
		}

#region IRemoteConfigLoader

		public ReadOnlyReactiveProperty<ERemoteConfigState> State { get; private set; }

#endregion

		private void StartFirebaseInitialize()
		{
			RemoteConfig remoteConfig				= new();
			Dictionary<string, object> defaults		= new();

			defaults.Add( RemoteConfigManager.ActualAppVersionKey,	remoteConfig.ActualAppVersion );
			defaults.Add( RemoteConfigManager.BannedAppVersionsKey,	JsonConvert.SerializeObject( remoteConfig.BannedAppVersions ) );

			FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync( defaults )
			.ContinueWithOnMainThread( task =>
			{
				Logger.Log( Logger.Module.RemoteConfig, "RemoteConfig configured and ready!");
				Fetch();
			} );

		}

		private Task Fetch()
		{
			Logger.Log( Logger.Module.RemoteConfig, "Fetching data...");
			Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync( TimeSpan.Zero );
			
			return fetchTask.ContinueWithOnMainThread(FetchComplete);
		}
		
		private void FetchComplete(Task fetchTask)
		{
			if (!fetchTask.IsCompleted)
			{
				Logger.Log( Logger.Module.RemoteConfig, "Retrieval hasn't finished.");
				return;
			}

			var remoteConfig	= FirebaseRemoteConfig.DefaultInstance;
			var info			= remoteConfig.Info;

			if (info.LastFetchStatus != LastFetchStatus.Success)
			{
				Logger.Log( Logger.Module.RemoteConfig, $"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
				return;
			}

			// Fetch successful. Parameter values must be activated to use.
			remoteConfig.ActivateAsync().ContinueWithOnMainThread( task => {
				Logger.Log( Logger.Module.RemoteConfig, $"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");

				_initializeComplete.Execute();
			});
		}
	}
}
