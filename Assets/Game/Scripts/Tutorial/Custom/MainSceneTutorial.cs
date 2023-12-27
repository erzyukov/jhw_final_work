namespace Game.Tutorial
{
	using Game.Core;
	using Game.Profiles;
	using Game.Utilities;
	using Zenject;
	using UniRx;
	using System;
	using Game.Ui;

	public class MainSceneTutorial : ControllerBase, IInitializable
	{
		[Inject] private GameProfile _profile;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private IFingerHint _fingerHint;
		[Inject] private IDialogHint _dialogHint;
		[Inject] private ILocalizator _localizator;
		[Inject] private IUiLobbyScreen _uiLobbyScreen;

		private const int BattleHintLevelNumber = 2;
		private const string BattleHintStringCode = "battleHintGoToSecondBattle";
		
		private IDisposable _initBattleHint;

		public void Initialize()
		{
			if (_profile.Tutorial.IsBattleHintComplete.Value == false)
			{
				_initBattleHint = Observable.CombineLatest(
						_gameCycle.State,
						_profile.LevelNumber,
						(state, level) => (state, level)
					)
					.Where(v => v.state == GameState.Lobby && v.level == BattleHintLevelNumber)
					.Subscribe(_ => OnLobbyWithBattleHintLoaded())
					.AddTo(this);
			}
		}

		private void OnLobbyWithBattleHintLoaded()
		{
			_initBattleHint.Dispose();
			
			_fingerHint.Show(FingerPlace.LobbyBattle);
			_dialogHint.SetMessage(_localizator.GetString(BattleHintStringCode));
			_dialogHint.SetPlace(DialogHintPlace.Middle);
			_dialogHint.SetActive(true);

			_initBattleHint = _uiLobbyScreen.PlayButtonClicked
				.Subscribe(_ => OnPlayButtonClicked())
				.AddTo(this);
		}

		private void OnPlayButtonClicked()
		{
			_initBattleHint.Dispose();
			
			_fingerHint.Hide();
			_dialogHint.SetActive(false);
			
			_profile.Tutorial.IsBattleHintComplete.Value = true;
		}
	}
}