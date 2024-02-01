namespace Game.Ui
{
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;
	using Zenject;

	public class UiGameSettingsButton : MonoBehaviour
	{
		[SerializeField] private Button _button;

		[Inject] IUiGameSettingsWindow _window;

		private void Awake()
		{
			_button.OnClickAsObservable()
				.Subscribe(_ => _window.SetActive(true))
				.AddTo(this);
		}
	}
}