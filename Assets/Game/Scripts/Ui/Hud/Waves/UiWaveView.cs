namespace Game.Ui
{
	using UnityEngine;
	using TMPro;

	public interface IUiWaveView
	{
		Transform Transform { get; }
		RectTransform Ancor { get; }
		void SetSelectionIconActive(bool value);
		void SetNotAchievedIconActive(bool value);
		void SetCurrentIconActive(bool value);
		void SetCompleteIconActive(bool value);
		void SetTitle(string value);
		void SetActive(bool value);
	}

    public class UiWaveView : MonoBehaviour, IUiWaveView
	{
		[SerializeField] private RectTransform _ancor;
		[SerializeField] private GameObject _selection;
		[SerializeField] private GameObject _notAchievedIcon;
		[SerializeField] private GameObject _currentIcon;
		[SerializeField] private GameObject _completeIcon;
		[SerializeField] private TextMeshProUGUI _title;

		#region IUiWaveView

		public Transform Transform => transform;

		public RectTransform Ancor => _ancor;

		public void SetSelectionIconActive(bool value) =>
			_selection.SetActive(value);

		public void SetNotAchievedIconActive(bool value) =>
			_notAchievedIcon.SetActive(value);

		public void SetCurrentIconActive(bool value) =>
			_currentIcon.SetActive(value);

		public void SetCompleteIconActive(bool value) =>
			_completeIcon.SetActive(value);

		public void SetTitle(string value) =>
			_title.text = value;

		public void SetActive(bool value) =>
			gameObject.SetActive(value);

		#endregion
	}
}