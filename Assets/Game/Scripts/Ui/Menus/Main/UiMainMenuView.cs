namespace Game.Ui
{
	using Game.Core;
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;

	public interface IUiMainMenuView
	{
		List<GameState> ActiveOnGameState { get; }
		List<UiMainMenuButton> Buttons { get; }
		void SetActive(bool value);
		void SetButtonLocked(GameState screen);
		void SetButtonActive(GameState screen);
		void SetButtonSelected(GameState screen);
	}

	public class UiMainMenuView : MonoBehaviour, IUiMainMenuView
	{
		[SerializeField] private Sprite _lockedIcon;
		[SerializeField] private List<GameState> _activeOnGameState;
		[SerializeField] private List<UiMainMenuButton> _buttons;
		[SerializeField] private Color _lockedButtonColor;
		[SerializeField] private Color _activeButtonColor;
		[SerializeField] private Color _selectedButtonColor;
		[SerializeField] private float _defaultButtonHeight;
		[SerializeField] private float _selectedButtonHeight;

		#region IUiMainMenuView

		public List<GameState> ActiveOnGameState => _activeOnGameState;

		public List<UiMainMenuButton> Buttons => _buttons;

		public void SetActive(bool value) => gameObject.SetActive(value);

		public void SetButtonLocked(GameState state)
		{
			UiMainMenuButton button = _buttons.Where(button => button.TargetGameState == state).First();

			if (button == null)
				return;

			button.SetBackgroundColor(_lockedButtonColor);
			button.SetIcon(_lockedIcon);
			button.SetTitleActive(false);
			button.SetInteractable(false);
			button.SetHeight(_defaultButtonHeight);
		}

		public void SetButtonActive(GameState state)
		{
			UiMainMenuButton button = _buttons.Where(button => button.TargetGameState == state).First();

			if (button == null)
				return;

			button.SetDefaultIcon();
			button.SetBackgroundColor(_activeButtonColor);
			button.SetTitleActive(false);
			button.SetInteractable(true);
			button.SetHeight(_defaultButtonHeight);
		}

		public void SetButtonSelected(GameState state)
		{
			UiMainMenuButton button = _buttons.Where(button => button.TargetGameState == state).FirstOrDefault();
			
			if (button == null)
				return;

			button.SetBackgroundColor(_selectedButtonColor);
			button.SetTitleActive(true);
			button.SetInteractable(false);
			button.SetHeight(_selectedButtonHeight);
		}

		#endregion

	}
}