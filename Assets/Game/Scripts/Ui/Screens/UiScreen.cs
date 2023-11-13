namespace Game.UI
{
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public enum Screen
	{
		None,
		Lobby,
		Win,
		Lose,
	}

	public interface IUiScreen
	{
		ReactiveCommand Closed { get; }
		Screen Screen { get; }
		void SetActive(bool value);
	}


	public abstract class UiScreen : MonoBehaviour, IUiScreen
	{
		[SerializeField] private Button _closeButton;

		protected virtual void OnEnable() { }

		protected virtual void Start()
		{
			if (_closeButton != null)
			{
				_closeButton.OnClickAsObservable()
					.Subscribe(_ => OnCloseHandler())
					.AddTo(this);
			}
		}

		protected virtual void OnCloseHandler()
		{
			SetActive(false);
			Closed.Execute();
		}

		#region IUiScreen implementation

		public ReactiveCommand Closed { get; } = new ReactiveCommand();

		public abstract Screen Screen { get; }

		public void SetActive(bool show) => gameObject.SetActive(show);

		#endregion
	}
}
