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

		// TODO: Refact: redo to passive view

		void Start()
        {
			SetPowerActive(_unitData.Power.Value != 0);

			_gameCycle.State
				.Subscribe(OnGameStateChange)
				.AddTo(this);

			_health.HealthRatio
				.Subscribe(OnHealthRatioChangeHandler)
			.AddTo(this);

			_uiHealthCanvas.localPosition = _uiHealthCanvas.localPosition.WithY(_unitData.RendererHeight);

			_unitData.Power
				.Subscribe(OnPowerChanged)
				.AddTo(this);

			Observable.Merge(
				_draggable.Dragging.Select(_ => false),
				_draggable.Dropped.Select(_ => true)
			)
				.Subscribe(SetDraggingState)
				.AddTo(this);

			_unitData.SupposedPower
				.Subscribe(OnSupposedPowerChanged)
				.AddTo(this);

			_grade.sprite = _unitsConfig.GradeSprites[_unitData.GradeIndex];
		}

		private void OnPowerChanged(int value)
		{
			SetPowerValue(value);
			SetPowerActive(value != 0);
		}

		private void SetDraggingState(bool value)
		{
			SetPowerActive(value);
			SetGradeActive(value);
		}

		private void OnSupposedPowerChanged(int value)
		{
			SetPowerActive(value != 0);
			SetPowerValue((value != 0) ? value: _unitData.Power.Value);
			_power.color = (value != 0) ? _supposedPowerColor: _defaultPowerColor;
		}

		private void OnHealthRatioChangeHandler(float ratio)
		{
			_slider.value = ratio;
			OnGameStateChange(_gameCycle.State.Value);
		}

		private void OnGameStateChange(GameState gameState)
		{
			bool isSliderActive = _slider.value != 0 && gameState == GameState.BattleStage;
			_slider.gameObject.SetActive(isSliderActive);

			bool isTacticalStage = gameState == GameState.TacticalStage;
			SetPowerActive(isTacticalStage);
			SetGradeActive(_unitData.IsHero && isTacticalStage);
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