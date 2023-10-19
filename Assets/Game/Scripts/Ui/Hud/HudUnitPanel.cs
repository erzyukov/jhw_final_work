namespace Game.Ui
{
	using System;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;
	using Unit = Units.Unit;

	public interface IHudUnitPanel
	{
		ReactiveCommand<Unit.Kind> UnitSelectButtonPressed { get; }
	}

	public class HudUnitPanel : MonoBehaviour, IHudUnitPanel
	{
		[SerializeField] private CommandButton[] _commandButtons;

		public ReactiveCommand<Unit.Kind> UnitSelectButtonPressed { get; } = new ReactiveCommand<Unit.Kind>();

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
			public Unit.Kind UnitType;
			public Button Button;
		}
    }
}