namespace Game.Units
{
	using Game.Core;
	using Game.Utilities;
	using UnityEngine;
	using Zenject;
	using UniRx;

	public interface IUnitPosition
	{
		void ResetPosition();

	}

	public class UnitPosition : ControllerBase, IUnitPosition, IInitializable
	{

		[Inject] private IUnitView _unitView;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private UnitCreateData _unitCreateData;

		private Quaternion _defaultRotation;

		public void Initialize()
		{
			_defaultRotation = Quaternion.identity;

			_gameCycle.State
				.Where(s => s == GameState.TacticalStage || s == GameState.BattleStage)
				.Subscribe(OnGameStateChange)
				.AddTo(this);
		}

		private void OnGameStateChange(GameState state)
		{
			if (state == GameState.TacticalStage && _unitCreateData.IsHero == true)
				_defaultRotation = Quaternion.Euler(0, 180, 0);
			else
				_defaultRotation = Quaternion.identity;

			ResetPosition();
		}

		#region IUnitPosition

		public void ResetPosition()
		{
			_unitView.Transform.localPosition = Vector3.zero;
			_unitView.Transform.localRotation = _defaultRotation;
		}

		#endregion

	}
}
