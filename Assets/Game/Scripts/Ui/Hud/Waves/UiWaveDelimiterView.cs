namespace Game.Ui
{
	using UnityEngine;

	public interface IUiWaveDelimiterView
	{
		Transform Transform { get; }
		void SetCurrentIconActive(bool value);
		void SetDefaultIconActive(bool value);
		void SetActive(bool value);
	}


	public class UiWaveDelimiterView : MonoBehaviour, IUiWaveDelimiterView
	{
		[SerializeField] private GameObject _defaultIcon;
		[SerializeField] private GameObject _activeIcon;

		#region IUiWaveDelimiterView

		public Transform Transform => transform;

		public void SetCurrentIconActive(bool value) =>
			_defaultIcon.SetActive(value);

		public void SetDefaultIconActive(bool value) =>
			_activeIcon.SetActive(value);

		public void SetActive(bool value) =>
			gameObject.SetActive(value);

		#endregion
	}
}
