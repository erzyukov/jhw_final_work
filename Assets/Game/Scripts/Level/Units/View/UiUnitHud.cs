namespace Game.Units
{
	using UnityEngine;
	using UnityEngine.UI;
	using Zenject;
	using UniRx;
	using Game.Core;
	using Game.Utilities;
	using TMPro;
	using Game.Configs;

	public class UiUnitHud : MonoBehaviour
    {
		[SerializeField] private Slider _slider;
		[SerializeField] private Transform _uiHealthCanvas;
		[SerializeField] private TextMeshProUGUI _power;
		[SerializeField] private Image _grade;
		[SerializeField] private Color _defaultPowerColor;
		[SerializeField] private Color _supposedPowerColor;

		[Inject] private IUnitHealth _health;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IUnitData _unitData;
		[Inject] private IDraggable _draggable;
		[Inject] private UnitsConfig _unitsConfig;
		[Inject] private UnitConfig _unitConfig;

		// TODO: Refact: redo to passive view

		void Start()
        {
			_gameCycle.State
				.Subscribe(OnGameStateChange)
				.AddTo(this);

			_health.HealthRatio
				.Subscribe(OnHealthRatioChangeHandler)
				.AddTo(this);

			_uiHealthCanvas.localPosition = _uiHealthCanvas.localPosition.WithY(_unitData.RendererHeight);

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

			if (_unitConfig.IsDebug)
			{
				_unitData.SupposedPower
					.Subscribe(v => Debug.LogWarning($"==> SupposedPower: {v}"))
					.AddTo(this);

				_unitData.Power
					.Subscribe(v => Debug.LogWarning($"==> Power: {v}"))
					.AddTo(this);
			}

			_grade.sprite = _unitsConfig.GradeSprites[_unitData.GradeIndex];
		}

		private void UpdatePowerValue()
		{
			int supposedPower = _unitData.SupposedPower.Value;
			int actualPower = _unitData.Power.Value;
			bool isSupposed = supposedPower != 0;
			int value = isSupposed ? supposedPower : actualPower;
			_power.color = (isSupposed) ? _supposedPowerColor : _defaultPowerColor;
			SetPowerValue(value);
			SetPowerActive(value != 0);
		}

		private void SetDraggingState(bool value)
		{
			SetPowerActive(value);
			SetGradeActive(value);
		}

		private void OnHealthRatioChangeHandler(float ratio)
		{
			_slider.value = ratio;
			UpdateSliderActive();
		}

		private void OnGameStateChange(GameState gameState)
		{
			UpdateSliderActive();

			bool isTacticalStage = gameState == GameState.TacticalStage;
			SetPowerActive(isTacticalStage);
			SetGradeActive(_unitData.IsHero && isTacticalStage);
		}

		private void UpdateSliderActive()
		{
			bool isSliderActive = _slider.value != 0 && _gameCycle.State.Value == GameState.BattleStage;
			_slider.gameObject.SetActive(isSliderActive);
		}

		private void SetPowerActive(bool value)
		{
			bool isActive = value && (_unitData.Power.Value != 0 || _unitData.SupposedPower.Value != 0);
			_power.transform.parent.gameObject.SetActive(isActive);
		}

		private void SetGradeActive(bool value) =>
			_grade.gameObject.SetActive(value);

		private void SetPowerValue(int value) =>
			_power.text = value.ToString();
	}
}