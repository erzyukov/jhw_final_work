namespace Game.Ui
{
	using UnityEngine;
	using TMPro;
	using UnityEngine.UI;

	public interface IUiLoseScreen : IUiScreen
	{
		void SetLevelReward(int value);
		void SetHeroLevel(int value);
		void SetExperienceRatio(float value);
	}

	public class UiLoseScreen : UiScreen, IUiLoseScreen
	{
		[SerializeField] private TextMeshProUGUI _levelReward;
		[SerializeField] private TextMeshProUGUI _heroLevel;
		[SerializeField] private Slider _experienceRatio;

		public override Screen Screen => Screen.Lose;

		#region IUiLoseScreen

		public void SetLevelReward(int value) => _levelReward.text = value.ToString();

		public void SetHeroLevel(int value) => _heroLevel.text = value.ToString();

		public void SetExperienceRatio(float value) => _experienceRatio.value = value;

		#endregion
	}
}