namespace Game
{
	using Game.Units;
	using UnityEngine;
	using UnityEngine.UI;
	using Zenject;
	using UniRx;
	using Game.Core;

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

			_health.HealthRate
				.Subscribe(rate => _slider.value = rate)
				.AddTo(this);
		}
    }
}