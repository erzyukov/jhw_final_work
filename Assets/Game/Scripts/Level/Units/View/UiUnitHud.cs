namespace Game.Units
{
	using UnityEngine;
	using UnityEngine.UI;
	using Zenject;
	using UniRx;
	using Game.Core;
	using Game.Utilities;

	public class UiUnitHud : MonoBehaviour
    {
		[SerializeField] Slider _slider;
		[SerializeField] private Transform _uiHealthCanvas;

		[Inject] private IUnitHealth _health;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IUnitData _unitData;

		void Start()
        {
			_gameCycle.State
				.Subscribe(_ => UpdateSliderActive())
				.AddTo(this);

			_health.HealthRatio
				.Subscribe(OnHealthRatioChangeHandler)
			.AddTo(this);

			_uiHealthCanvas.localPosition = _uiHealthCanvas.localPosition.WithY(_unitData.RendererHeight);
		}

		private void OnHealthRatioChangeHandler(float ratio)
		{
			_slider.value = ratio;
			UpdateSliderActive();
		}

		private void UpdateSliderActive()
		{
			bool isActive = _slider.value != 0 && _gameCycle.State.Value == GameState.BattleStage;
			_slider.gameObject.SetActive(isActive);
		}
	}
}