namespace Game.Ui
{
	using System;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;
	using Unit = Units.Unit;

	public interface IHudUnitPanel
	{
		ReactiveCommand<Unit.Type> UnitSelectButtonPressed { get; }
	}

	public class HudUnitPanel : MonoBehaviour, IHudUnitPanel
	{
		[SerializeField] private CommandButton[] _commandButtons;

		public ReactiveCommand<Unit.Type> UnitSelectButtonPressed { get; } = new ReactiveCommand<Unit.Type>();

		private void Awake()
		{
            foreach (var commandButton in _commandButtons)
            {
				commandButton.Button.OnClickAsObservable()
					.Subscribe(_ => UnitSelectButtonPressed.Execute(commandButton.UnitType))
					.AddTo(this);
			}
        }

		[Serializable]
		public struct CommandButton
		{
			public Unit.Type UnitType;
			public Button Button;
		}
    }
}