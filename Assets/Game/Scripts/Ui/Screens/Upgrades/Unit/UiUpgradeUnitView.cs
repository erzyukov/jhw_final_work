namespace Game.Ui
{
	using UnityEngine;
	using Zenject;

	public interface IUiUpgradeUnitView
	{

	}

	public class UiUpgradeUnitView : MonoBehaviour, IUiUpgradeUnitView
	{



		public class Factory : PlaceholderFactory<UiUpgradeUnitViewFactory.Args, IUiUpgradeUnitView> { }
	}
}
