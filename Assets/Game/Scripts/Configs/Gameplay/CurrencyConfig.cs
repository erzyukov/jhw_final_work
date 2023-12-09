namespace Game.Configs
{
	using UnityEngine;

	[CreateAssetMenu(fileName = "Currency", menuName = "Configs/Currency", order = (int)Config.Currency)]
	public class CurrencyConfig : ScriptableObject
	{
		[Header("Tactical Stage")]
		[SerializeField] private int _unitSummonPrice;

		public int UnitSummonPrice => _unitSummonPrice;
	}
}