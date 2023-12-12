namespace Game.Ui
{
	using UnityEngine;
	using UnityEngine.UI;

	public class UiMainMenuButton : MonoBehaviour
    {
		[SerializeField] private Screen _targetScreen;

		public Screen TargetScreen => _targetScreen;
	}
}