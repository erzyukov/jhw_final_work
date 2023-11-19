namespace Game.Ui
{
	using Game.Core;
	using UnityEngine;
	using UniRx;

	public interface IUiHudPartition
	{
		ReactiveCommand Opening { get; }
		GameState[] ActiveInGameStates { get; }
		void SetActive(bool value);
	}

	public class UiHudPartition : MonoBehaviour, IUiHudPartition
	{
		[SerializeField] private GameState[] _activeInGameStates;

		#region IUiHud

		public ReactiveCommand Opening { get; } = new ReactiveCommand();

		public GameState[] ActiveInGameStates => _activeInGameStates;

		public void SetActive(bool value)
		{
			if (value)
				Opening.Execute();

			gameObject.SetActive(value);
		}

		#endregion
	}
}