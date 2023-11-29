namespace Game.Ui
{
	using UnityEngine;
	using TMPro;

	public interface IUiBattleStageHud : IUiHudPartition
	{
		void SetHeroUnitsCount(int value);
		void SetEnemyUnitsCount(int value);
	}

	public class UiBattleStageHud : UiHudPartition, IUiBattleStageHud
	{
		[SerializeField] private TextMeshProUGUI _heroUnitsCount;
		[SerializeField] private TextMeshProUGUI _enemyUnitsCount;

		#region IUiTacticalStageHud

		public void SetHeroUnitsCount(int value) => _heroUnitsCount.text = value.ToString();
		
		public void SetEnemyUnitsCount(int value) => _enemyUnitsCount.text = value.ToString();

		#endregion
	}
}