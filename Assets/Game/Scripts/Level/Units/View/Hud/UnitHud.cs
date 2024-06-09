namespace Game.Units
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Configs;
	using Game.Core;
	using UnityEngine;

	public class UnitHud : ControllerBase, IInitializable
	{
		[Inject] private IUiUnitHud _uiHud;
		[Inject] private IUnitHealth _health;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IUnitData _unitData;
		[Inject] private IDraggable _draggable;
		[Inject] private UnitsConfig _unitsConfig;
		[Inject] private UnitConfig _unitConfig;

		public void Initialize()
		{
			_uiHud.Initialized
				.Subscribe(_ => OnHudInitialized())
				.AddTo(this);
		}

		private void OnHudInitialized()
		{
			_uiHud.SetHealthCanvasHeight(_unitData.RendererHeight);

			_gameCycle.State
				.Subscribe(OnGameStateChange)
				.AddTo(this);

			_health.HealthRatio
				.Subscribe(OnHealthRatioChangeHandler)
				.AddTo(this);

			Observable.Merge(
				_unitData.SupposedPower,
				_unitData.Power
			)
				.Subscribe(_ => UpdatePowerValue())
				.AddTo(this);

			Observable.Merge(
				_draggable.Dragging.Select(_ => false),
				_draggable.Dropped.Select(_ => true)
			)
				.Subscribe(SetDraggingState)
				.AddTo(this);

			int gradeIndex = (_unitData.GradeIndex < _unitsConfig.GradeSprites.Length) 
				? _unitData.GradeIndex
				: 0;
			_uiHud.SetGradeSprite(_unitsConfig.GradeSprites[gradeIndex]);

			if (_unitData.IsHero)
				_uiHud.SetHeroHealthColor();
			else
				_uiHud.SetEnemyHealthColor();

			if (_unitConfig.IsDebug)
			{
				_unitData.SupposedPower
					.Subscribe(v => Debug.LogWarning($"==> SupposedPower: {v}"))
					.AddTo(this);

				_unitData.Power
					.Subscribe(v => Debug.LogWarning($"==> Power: {v}"))
					.AddTo(this);
			}
		}

		private void OnHealthRatioChangeHandler(float ratio)
		{
			_uiHud.SetHealthRatio(ratio);
			UpdateSliderActive();
		}

		private void OnGameStateChange(GameState gameState)
		{
			UpdateSliderActive();

			bool isTacticalStage = gameState == GameState.TacticalStage;
			SetPowerActive(isTacticalStage);
			_uiHud.SetGradeActive(_unitData.IsHero && isTacticalStage);
		}

		private void UpdateSliderActive()
		{
			bool isSliderActive = _health.HealthRatio.Value != 0 && _gameCycle.State.Value == GameState.BattleStage;
			_uiHud.SetHealthBarActive(isSliderActive);
		}

		private void UpdatePowerValue()
		{
			int supposedPower = _unitData.SupposedPower.Value;
			int actualPower = _unitData.Power.Value;
			bool isSupposed = supposedPower != 0;
			int value = isSupposed ? supposedPower : actualPower;
			_uiHud.SetSupposedState(isSupposed);
			_uiHud.SetPowerValue(value);
			SetPowerActive(value != 0);
		}

		private void SetPowerActive(bool value)
		{
			bool isActive = value && (_unitData.Power.Value != 0 || _unitData.SupposedPower.Value != 0);
			_uiHud.SetPowerActive(isActive);
		}

		private void SetDraggingState(bool value)
		{
			SetPowerActive(value);
			_uiHud.SetGradeActive(value);
		}
	}
}
