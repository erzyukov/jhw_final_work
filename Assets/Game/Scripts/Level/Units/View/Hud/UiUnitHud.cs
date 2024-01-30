namespace Game.Units
{
	using UnityEngine;
	using UnityEngine.UI;
	using UniRx;
	using Game.Utilities;
	using TMPro;

	public interface IUiUnitHud
	{
		ReactiveCommand Initialized { get; }
		void SetPowerActive(bool value);
		void SetPowerValue(int value);
		void SetSupposedState(bool value);
		void SetHealthCanvasHeight(float value);
		void SetHealthBarActive(bool value);
		void SetHealthRatio(float value);
		void SetGradeActive(bool value);
		void SetGradeSprite(Sprite value);
	}

	public class UiUnitHud : MonoBehaviour, IUiUnitHud
	{
		[SerializeField] private Slider _slider;
		[SerializeField] private Transform _uiHealthCanvas;
		[SerializeField] private TextMeshProUGUI _power;
		[SerializeField] private Image _grade;
		[SerializeField] private Color _defaultPowerColor;
		[SerializeField] private Color _supposedPowerColor;

		void Start() =>
			Initialized.Execute();

		#region MyRegion

		public ReactiveCommand Initialized { get; } = new ReactiveCommand();

		public void SetPowerActive(bool value) =>
			_power.transform.parent.gameObject.SetActive(value);

		public void SetPowerValue(int value) =>
			_power.text = value.ToString();

		public void SetSupposedState(bool value) =>
			_power.color = (value) ? _supposedPowerColor : _defaultPowerColor;

		public void SetHealthCanvasHeight(float value) =>
			_uiHealthCanvas.localPosition = _uiHealthCanvas.localPosition.WithY(value);

		public void SetHealthBarActive(bool value) =>
			_slider.gameObject.SetActive(value);

		public void SetHealthRatio(float value) =>
			_slider.value = value;

		public void SetGradeActive(bool value) =>
			_grade.gameObject.SetActive(value);

		public void SetGradeSprite(Sprite value) =>
			_grade.sprite = value;

		#endregion
	}
}