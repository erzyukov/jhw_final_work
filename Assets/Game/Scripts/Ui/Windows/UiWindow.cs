namespace Game.Ui
{
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public enum Window
	{
		BattleMenu,
		ContinueLevel,
		GameSettings,
	}

	public interface IUiWindow
	{
		ReactiveCommand Opened { get; }
		ReactiveCommand Closed { get; }
		Window Type { get; }
		void SetActive(bool value);
	}

	public abstract class UiWindow : MonoBehaviour, IUiWindow
	{
		[SerializeField] private Button _closeButton;

		protected virtual void Start()
		{
			_closeButton.OnClickAsObservable()
				.Subscribe(_ => OnCloseButtonClicked())
				.AddTo(this);
		}

		private void OnCloseButtonClicked()
		{
			SetActive(false);
			Closed.Execute();
		}

		#region IUiWindow

		public ReactiveCommand Opened { get; } = new ReactiveCommand();
		public ReactiveCommand Closed { get; } = new ReactiveCommand();
		public abstract Window Type { get; }

		public void SetActive(bool value)
		{
			gameObject.SetActive(value);

			if (value)
				Opened.Execute();
		}

		#endregion
	}
}
