namespace Game.Analytics
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Core;
	using System.Linq;
	using GameAnalyticsSDK;

	public class ProgressionHierarchyAnalytics : HierarchyAnalyticsBase, IInitializable
	{
		[Inject] private IGameLevel _gameLevel;

		public void Initialize()
		{
			Observable.Merge(
				_gameLevel.WaveStarted.Select(_ => GAProgressionStatus.Start),
				_gameLevel.WaveFinished.Where(v => v == GameLevel.Result.Win).Select(_ => GAProgressionStatus.Complete),
				_gameLevel.WaveFinished.Where(v => v == GameLevel.Result.Fail).Select(_ => GAProgressionStatus.Fail)
			)
				.Subscribe(OnProgressionChanged)
				.AddTo(this);
		}

		private void OnProgressionChanged(GAProgressionStatus status)
		{
			SendProgressionEvent(status, BattleDefaultTypeEventKey, LevelKey, WaveKey);
		}
	}
}