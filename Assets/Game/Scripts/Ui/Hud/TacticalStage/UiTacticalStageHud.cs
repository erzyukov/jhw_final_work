namespace Game.Ui
{
	using UnityEngine;
	using TMPro;
	using UnityEngine.UI;
	using System;
	using UniRx;
	using Game.Tutorial;

	public interface IUiTacticalStageHud : IUiHudPartition
	{
		IObservable<Unit> SummonButtonClicked { get; }
		IObservable<Unit> StartBattleButtonClicked { get; }
		HintedButton.Parameters SummonButtonHintParameters { get; }
		void SetSummonPrice(int value);
		void SetSummonButtonActive(bool value);
		void SetStartBattleButtonActive(bool value);
		void SetSummonButtonInteractable(bool value);
		void SetStartBattleButtonInteractable(bool value);
	}

	public class UiTacticalStageHud : UiHudPartition, IUiTacticalStageHud
	{
		[SerializeField] private TextMeshProUGUI _summonPrice;
		[SerializeField] private Button _summonButton;
		[SerializeField] private HintedButton _hintedButton;
		[SerializeField] private Button _startBattleButton;

		#region IUiTacticalStageHud

		public IObservable<Unit> SummonButtonClicked => _summonButton.OnClickAsObservable();

		public IObservable<Unit> StartBattleButtonClicked => _startBattleButton.OnClickAsObservable();

		public HintedButton.Parameters SummonButtonHintParameters => _hintedButton.HintParameters;

		public void SetSummonPrice(int value) => _summonPrice.text = value.ToString();

		public void SetSummonButtonActive(bool value) => _summonButton.gameObject.SetActive(value);

		public void SetStartBattleButtonActive(bool value) => _startBattleButton.gameObject.SetActive(value);

		public void SetSummonButtonInteractable(bool value) => _summonButton.interactable = value;

		public void SetStartBattleButtonInteractable(bool value) => _startBattleButton.interactable = value;

		#endregion
	}
}