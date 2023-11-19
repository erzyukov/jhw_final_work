namespace Game.Ui
{
	using UnityEngine;
	using TMPro;

	public interface IUiCommonGameplayHud : IUiHudPartition
	{
		void SetSummonCurrencyAmount(int value);
		void SetSoftCurrencyAmount(int value);
	}

	public class UiCommonGameplayHud : UiHudPartition, IUiCommonGameplayHud
	{
		[SerializeField] private TextMeshProUGUI _summonCurrencyAmount;
		[SerializeField] private TextMeshProUGUI _softCurrencyAmount;

		#region IUiTacticalStageHud

		public void SetSummonCurrencyAmount(int value) => _summonCurrencyAmount.text = value.ToString();

		public void SetSoftCurrencyAmount(int value) => _softCurrencyAmount.text = value.ToString();

		#endregion
	}
}