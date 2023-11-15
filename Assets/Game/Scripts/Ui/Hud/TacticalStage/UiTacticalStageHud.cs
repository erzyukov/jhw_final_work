namespace Game.Ui
{
	using UnityEngine;
	using TMPro;
	using UnityEngine.UI;
	using System;
	using UniRx;

	public interface IUiTacticalStageHud : IUiHudPartition
	{
		IObservable<Unit> SummonButtonClicked { get; }
		IObservable<Unit> StartBattleButtonClicked { get; }
		void SetSummonPrice(int value);
	}

	public class UiTacticalStageHud : UiHudPartition, IUiTacticalStageHud
	{
		[SerializeField] private TextMeshProUGUI _summonPrice;
		[SerializeField] private Button _summonButton;
		[SerializeField] private Button _startBattleButton;

		#region IUiTacticalStageHud

		public IObservable<Unit> SummonButtonClicked => _summonButton.OnClickAsObservable();

		public IObservable<Unit> StartBattleButtonClicked => _startBattleButton.OnClickAsObservable();

		public void SetSummonPrice(int value) => _summonPrice.text = value.ToString();

		#endregion
	}
}