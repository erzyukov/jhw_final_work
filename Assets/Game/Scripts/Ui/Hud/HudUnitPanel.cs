namespace Game.Ui
{
	using Units;
	using System;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public interface IHudUnitPanel
	{
		ReactiveCommand<CombatUnit.Type> UnitSelectButtonPressed { get; }
	}

	public class HudUnitPanel : MonoBehaviour, IHudUnitPanel
	{
		[SerializeField] private CommandButton[] _commandButtons;

		public ReactiveCommand<CombatUnit.Type> UnitSelectButtonPressed { get; } = new ReactiveCommand<CombatUnit.Type>();

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
			public CombatUnit.Type CombatUnitType;
			public Button Button;
		}
    }
}
