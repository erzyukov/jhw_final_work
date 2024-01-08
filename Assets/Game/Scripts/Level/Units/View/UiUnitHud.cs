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

		[Inject] private IUnitHealth _health;
		[Inject] private IGameCycle _gameCycle;

		void Start()
        {
			_gameCycle.State
				.Subscribe(_ => UpdateSliderActive())
				.AddTo(this);

			_health.HealthRatio
				.Subscribe(OnHealthRatioChangeHandler)
				.AddTo(this);

			//_slider.transform.localPosition = _slider.transform.localPosition.WithZ(_unitData.RendererHeight);
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