namespace Game
{
	using Game.Units;
	using UnityEngine;
	using UnityEngine.UI;
	using Zenject;
	using UniRx;
	using Game.Core;
	using Game.Utilities;

	public class UiUnitHud : MonoBehaviour
    {
		[SerializeField] Slider _slider;

		[Inject] IUnitHealth _health;
		[Inject] IGameCycle _gameCycle;

		void Start()
        {
			_gameCycle.State
				.Subscribe(_ => UpdateSliderActive())
				.AddTo(this);

			_health.HealthRatio
				.Subscribe(OnHealthRatioChangeHandler)
				.AddTo(this);
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