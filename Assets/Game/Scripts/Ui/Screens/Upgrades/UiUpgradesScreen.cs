namespace Game.Ui
{
	public interface IUiUpgradesScreen : IUiScreen
	{
	}

	public class UiUpgradesScreen : UiScreen, IUiUpgradesScreen
	{

		public override Screen Screen => Screen.Upgrades;

		#region IUiLobbyScreen

		#endregion
	}
}
