namespace Game.Ui
{
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public enum Screen
	{
		None,
		Loading,
		Lobby,
		Win,
		Lose,
		LevelReward,
		Upgrades,
	}

	public interface IUiScreen
	{
		ReactiveCommand Opening { get; }
		ReactiveCommand Opened { get; }
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

		public ReactiveCommand Opening { get; } = new ReactiveCommand();
		public ReactiveCommand Opened { get; } = new ReactiveCommand();
		public ReactiveCommand Closed { get; } = new ReactiveCommand();

		public abstract Screen Screen { get; }

		public void SetActive(bool value)
		{
			if (value)
				Opening.Execute();

			gameObject.SetActive(value);

			if (value)
				Opened.Execute();
		}

		#endregion
	}
}
