namespace Game.Ui
{
	using Units;
	using System;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public interface IHudUnitPanel
	{
		ReactiveCommand<Units.Unit.Type> UnitSelectButtonPressed { get; }
	}

	public class HudUnitPanel : MonoBehaviour, IHudUnitPanel
	{
		[SerializeField] private CommandButton[] _commandButtons;

		public ReactiveCommand<Units.Unit.Type> UnitSelectButtonPressed { get; } = new ReactiveCommand<Units.Unit.Type>();

		private void Awake()
		{
            foreach (var commandButton in _commandButtons)
            {
				commandButton.Button.OnClickAsObservable()
					.Subscribe(_ => UnitSelectButtonPressed.Execute(commandButton.CombatUnitType))
					.AddTo(this);
			}
        }

		[Serializable]
		public struct CommandButton
		{
			public Units.Unit.Type CombatUnitType;
			public Button Button;
		}
    }
}
