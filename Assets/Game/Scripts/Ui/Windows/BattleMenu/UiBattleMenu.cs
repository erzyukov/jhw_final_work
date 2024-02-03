namespace Game.Ui
{
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using Game.Core;

	public class UiBattleMenu : ControllerBase, IInitializable
	{
		[Inject] IUiBattleMenuWindow _window;
		[Inject] IGameLevel _gameLevel;

		public void Initialize()
		{
			Observable.Merge(
					_window.ContinueButtonClicked,
					_window.LobbyButtonClicked
				)
				.Subscribe(_ => _window.SetActive(false))
				.AddTo(this);

			_window.LobbyButtonClicked
				.Subscribe(_ => _gameLevel.LeaveBattle())
				.AddTo(this);
		}
	}
}
