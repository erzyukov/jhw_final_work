namespace Game.Analytics
{
	using Game.Core;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using UnityEngine;

	public class TechnicalHierarchyAnalytics : HierarchyAnalyticsBase, IInitializable
	{
		[Inject] private IScenesManager _scenesManager;

		private const string TechnicalEventKey = "Technical";

		private const string TechnicalStep_1 = "01_start_loading"; // start loading game resources
		private const string TechnicalStep_2 = "02_finished_loading"; // finish loading game resources
		private const string TechnicalStep_3 = "03_start_game_scene"; // main scene loaded
		private const string TechnicalStep_4 = "04_finished_game_scene_actions"; // closed all popups after main scene loaded

		public void Initialize()
		{
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

		private void OnScenesLoading() =>
			SendTechnicalStep(TechnicalStep_1);

		private void OnMainLoading() =>
			SendTechnicalStep(TechnicalStep_2);

		private void OnMainLoaded() =>
			SendTechnicalStep(TechnicalStep_3);

		private void SendTechnicalStep(string step)
		{
			SendDesignEvent($"{TechnicalEventKey}:{step}", Time.time);
		}
	}
}