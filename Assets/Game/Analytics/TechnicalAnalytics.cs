namespace Game.Analytics
{
	using Game.Core;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using System.Collections.Generic;
	using UnityEngine;
	using Game.Profiles;

	public class TechnicalAnalytics : AnalyticsBase, IInitializable
	{
		[Inject] private IScenesManager _scenesManager;
		[Inject] private IApplicationPaused _applicationPaused;

		private const string AppPauseEventKey = "app_pause";
		private const string AppResumeEventKey = "app_resume";
		private const string TechnicalEventKey = "technical";
		
		private const string TechnicalStep_1 = "01_start_loading"; // start loading game resources
		private const string TechnicalStep_2 = "02_finished_loading"; // finish loading game resources
		private const string TechnicalStep_3 = "03_start_game_scene"; // main scene loaded
		private const string TechnicalStep_4 = "04_finished_game_scene_actions"; // closed all popups after main scene loaded

		private float _lastPauseEventTime;

		public void Initialize()
		{
			_lastPauseEventTime = Time.time;

			_applicationPaused.IsApplicationPaused
				.Skip(1)
				.Subscribe(OnApplicationPaused)
				.AddTo(this);

			_scenesManager.ResourceLoading
				.Subscribe(_ => OnScenesLoading())
				.AddTo(this);

			_scenesManager.MainLoading
				.Subscribe(_ => OnMainLoading())
				.AddTo(this);

			_scenesManager.MainLoaded
				.Subscribe(_ => OnMainLoaded())
				.AddTo(this);
		}

		private void OnApplicationPaused(bool value)
		{
			string key = value ? AppPauseEventKey : AppResumeEventKey;
			var properties = new Dictionary<string, object>
			{
				{ "session_time"   , Time.time },
				{ "time"    , Time.time - _lastPauseEventTime}
			};
			SendMessage(key, properties);

			_lastPauseEventTime = Time.time;
		}

		private void OnScenesLoading() =>
			SendTechnicalStep(TechnicalStep_1);

		private void OnMainLoading() =>
			SendTechnicalStep(TechnicalStep_2);

		private void OnMainLoaded() =>
			SendTechnicalStep(TechnicalStep_3);

		private void SendTechnicalStep(string step)
		{
			var properties = new Dictionary<string, object>
			{
				{ "step_name"   , step },
				{ "first_start" , GameProfile.Analytics.IsFirstRunApp}
			};
			SendMessage(TechnicalEventKey, properties, true);
		}
	}
}
