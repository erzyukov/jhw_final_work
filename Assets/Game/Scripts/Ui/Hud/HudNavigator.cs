namespace Game.Ui
{
	using Game.Core;
	using Game.Utilities;
	using System.Collections.Generic;
	using System.Linq;
	using Zenject;
	using UniRx;

	public class HudNavigator : ControllerBase, IInitializable
	{
		[Inject] private List<IUiHudPartition> _partitions;
		[Inject] private IGameCycle _gameCycle;

		public void Initialize()
		{
			_gameCycle.State
				.Subscribe(OnGameCycleStateChange)
				.AddTo(this);
		}

		private void OnGameCycleStateChange(GameState state)
		{
			foreach (var partition in _partitions)
			{
				bool isActive = partition.ActiveInGameStates.Contains(state);
				partition.SetActive(isActive);
			}
		}
	}
}