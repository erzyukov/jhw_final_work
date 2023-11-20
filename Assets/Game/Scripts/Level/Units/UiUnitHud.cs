namespace Game
{
	using Game.Units;
	using UnityEngine;
	using UnityEngine.UI;
	using Zenject;
	using UniRx;

    public class UiUnitHud : MonoBehaviour
    {
		[SerializeField] Slider _slider;

		[Inject] IUnitHealth _health;

        void Start()
        {
			_health.HealthRate
				.Subscribe(rate => _slider.value = rate)
				.AddTo(this);
		}
    }
}