namespace Game.Ui
{
	using Game.Configs;
	using Game.Core;
	using Game.Profiles;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;
	using Zenject;

	public class UiBattleMenuButton : MonoBehaviour
	{
		[SerializeField] private Button _button;

		[Inject] private IUiBattleMenuWindow _window;
		[Inject] private IGameCycle _gameCycle;
		[Inject] private GameProfile _gameProfile;
		[Inject] private MenuConfig _menuConfig;

		private void Awake()
		{
			_button.OnClickAsObservable()
				.Subscribe(_ => _window.SetActive(true))
				.AddTo(this);

			_gameCycle.State
				.Select(state => state == GameState.TacticalStage)
				.Subscribe(isButtonInteractiable => _button.interactable = isButtonInteractiable)
				.AddTo(this);

			_gameProfile.LevelNumber
				.Select(v => v >= _menuConfig.TacticalMenuActiveFromLevel)
				.Subscribe(isButtonActive => _button.gameObject.SetActive(isButtonActive))
				.AddTo(this);
		}
	}
}