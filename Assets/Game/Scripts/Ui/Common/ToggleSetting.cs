namespace Game.Ui
{
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public class ToggleSetting : MonoBehaviour
    {
		[SerializeField] private Toggle _toggle;
		[SerializeField] private GameObject _activeBackground;
		[SerializeField] private Image _checkmark;
		[SerializeField] private Sprite _enabledCheckmark;
		[SerializeField] private Sprite _disabledCheckmark;

		private void Awake()
		{
			_toggle.OnValueChangedAsObservable()
				.Subscribe(OnValueChanged)
				.AddTo(this);
		}

		private void OnValueChanged(bool value)
		{
			_activeBackground.SetActive(value);
			_checkmark.sprite = value ? _enabledCheckmark : _disabledCheckmark;
		}
	}
}