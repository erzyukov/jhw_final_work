namespace Game
{
	using Game.Units;
	using UnityEngine;
	using UnityEngine.UI;
	using Zenject;
	using UniRx;
	using Game.Core;
	using Game.Utilities;
	using Game.Ui;

	public class UiUnitHud : MonoBehaviour
    {
		[SerializeField] Slider _slider;

		[Inject] IUnitHealth _health;
		[Inject] IGameCycle _gameCycle;

		void Start()
        {
			_gameCycle.State
				.Select(state => state == GameState.BattleStage)
				.Subscribe(isBattleStage => _slider.gameObject.SetActive(isBattleStage))
				.AddTo(this);

			_health.HealthRatio
				.Subscribe(OnHealthRatioChangeHandler)
				.AddTo(this);
		}

		private void OnHealthRatioChangeHandler(float ratio)
		{
			_slider.value = ratio;
			_slider.gameObject.SetActive(ratio != 0);
		}
	}
}