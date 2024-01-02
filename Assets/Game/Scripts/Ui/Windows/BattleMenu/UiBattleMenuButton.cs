namespace Game.Ui
{
	using Game.Core;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;
	using Zenject;

	public class UiBattleMenuButton : MonoBehaviour
	{
		[SerializeField] private Button _button;

		[Inject] IUiBattleMenuWindow _window;
		[Inject] IGameCycle _gameCycle;

		private void Awake()
		{
			_button.OnClickAsObservable()
				.Subscribe(_ => _window.SetActive(true))
				.AddTo(this);

			_gameCycle.State
				.Select(state => state == GameState.TacticalStage)
				.Subscribe(isButtonActive => _button.interactable = isButtonActive)
				.AddTo(this);
		}
	}
}