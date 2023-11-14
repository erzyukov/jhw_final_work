namespace Game.Ui
{
	using Game.Core;
	using Game.Utilities;
	using UnityEngine;
	using DG.Tweening;
	using UnityEngine.UI;
	using Zenject;
	using UniRx;

    public class UiViel : MonoBehaviour
	{
		[SerializeField] private Image _veil;
		[SerializeField] private float _duration;

		[Inject] private IGameCycle _gameCycle;

		private void Start()
		{
			_gameCycle.State
				.Subscribe(OnGameCycleStateChanged)
				.AddTo(this);
		}

		private void OnGameCycleStateChanged(GameState state)
		{
			switch (state)
			{
				case GameState.LoadingLobby:
					SetActive(true);
					break;
				default: 
					SetActive(false);
					break;
			}
		}

		private void SetActive(bool value)
		{
			float alpha = value ? 1 : 0;
			_veil.DOFade(alpha, _duration);
		}
	}
}