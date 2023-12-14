namespace Game.Ui
{
	using UnityEngine;

	public interface IUiUpgradesScreen : IUiScreen
	{
		Transform UnitsContainer { get; }
		UiUnitUpgradeElement UnitElementPrefab { get; }
		GameObject UnitUnavailableDummyPrefab { get; }
	}

	public class UiUpgradesScreen : UiScreen, IUiUpgradesScreen
	{
		[SerializeField] private Transform _unitsContainer;
		[SerializeField] private UiUnitUpgradeElement _unitUpgradeElementPrefab;
		[SerializeField] private GameObject _unitUnavailableDummyPrefab;

		public override Screen Screen => Screen.Upgrades;

		#region IUiLobbyScreen

		public Transform UnitsContainer => _unitsContainer;

		public UiUnitUpgradeElement UnitElementPrefab => _unitUpgradeElementPrefab;

		public GameObject UnitUnavailableDummyPrefab => _unitUnavailableDummyPrefab;

		#endregion
	}
}