namespace Game.Ui
{
	using UnityEngine;
	using TMPro;

	public interface IUiLevelRewardScreen : IUiScreen
	{
		void SetSoftRewardAmount(int amount);
	}

	public class UiLevelRewardScreen : UiScreen, IUiLevelRewardScreen
	{
		[SerializeField] private TextMeshProUGUI _rewardAmount;

		public override Screen Screen => Screen.LevelReward;

		#region IUiLevelRewardScreen

		public void SetSoftRewardAmount(int value) => _rewardAmount.text = value.ToString();
		
		#endregion
	}
}