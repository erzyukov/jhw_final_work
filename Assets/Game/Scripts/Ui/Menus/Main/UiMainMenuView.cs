namespace Game.Ui
{
	using System.Collections.Generic;
	using UnityEngine;

	public interface IUiMainMenuView
	{
		List<UiMainMenuButton> Buttons { get; }
		void SetActive(bool value);
	}

    public class UiMainMenuView : MonoBehaviour, IUiMainMenuView
	{
		[SerializeField] private List<Screen> _activeOnScreens;
		[SerializeField] private List<UiMainMenuButton> _buttons;
		[SerializeField] private Color _lockedButtonColor;
		[SerializeField] private Color _activeButtonColor;
		[SerializeField] private Color _selectedButtonColor;

		#region IUiMainMenuView

		public List<UiMainMenuButton> Buttons => _buttons;

		public void SetActive(bool value) => gameObject.SetActive(value);

		#endregion

	}
}