namespace Game.Ui
{
	using Game.Core;
    using System.Collections.Generic;
    using System.Linq;
	using UnityEngine;

	public interface IUiMainMenuView
	{
		List<GameState> ActiveOnGameState { get; }
		List<UiMainMenuButton> Buttons { get; }
		UiMainMenuButton GetButton(GameState gameState);
		void SetActive(bool value);
		void SetButtonLocked(GameState gameState);
		void SetButtonActive(GameState gameState);
		void SetButtonSelected(GameState gameState);
		void SetButtonInteractable(GameState gameState, bool value);
    }

    public class UiMainMenuView : MonoBehaviour, IUiMainMenuView
	{
		[SerializeField] private Sprite _lockedIcon;
		[SerializeField] private List<GameState> _activeOnGameState;
		[SerializeField] private List<UiMainMenuButton> _buttons;
		[SerializeField] private Color _lockedButtonColor;
		[SerializeField] private Color _activeButtonColor;
		[SerializeField] private Color _selectedButtonColor;
		[SerializeField] private Color _lockedIconColor;
		[SerializeField] private Color _activeIconColor;
		[SerializeField] private Color _selectedIconColor;
		[SerializeField] private float _defaultButtonHeight;
		[SerializeField] private float _selectedButtonHeight;

        #region IUiMainMenuView

        public List<GameState> ActiveOnGameState => _activeOnGameState;

		public List<UiMainMenuButton> Buttons => _buttons;

		public UiMainMenuButton GetButton(GameState gameState) =>
			Buttons.Where(button => button.TargetGameState == gameState).FirstOrDefault();

		public void SetActive(bool value) => gameObject.SetActive(value);

		public void SetButtonLocked(GameState gameState)
		{
			UiMainMenuButton button = GetButton(gameState);

			if (button == null)
				return;

			button.SetBackgroundColor(_lockedButtonColor);
			button.SetIcon(_lockedIcon);
			button.SetIconColor(_lockedIconColor);
			button.SetTitleActive(false);
			button.SetInteractable(false);
			button.SetHeight(_defaultButtonHeight);
		}

		public void SetButtonActive(GameState gameState)
		{
			UiMainMenuButton button = GetButton(gameState);

			if (button == null)
				return;

			button.SetDefaultIcon();
			button.SetIconColor(_activeIconColor);
			button.SetBackgroundColor(_activeButtonColor);
			button.SetTitleActive(false);
			button.SetInteractable(true);
			button.SetHeight(_defaultButtonHeight);
		}

		public void SetButtonSelected(GameState gameState)
		{
			UiMainMenuButton button = GetButton(gameState);

			if (button == null)
				return;

			button.SetIconColor(_selectedIconColor);
			button.SetBackgroundColor(_selectedButtonColor);
			button.SetTitleActive(true);
			button.SetInteractable(false);
			button.SetHeight(_selectedButtonHeight);
		}

		public void SetButtonInteractable(GameState gameState, bool value)
		{
			UiMainMenuButton button = GetButton(gameState);

			if (button == null)
				return;

			button.SetInteractable(value);
		}

		#endregion

	}
}