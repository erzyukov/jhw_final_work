namespace Game.Ui
{
	public interface IUiWinScreen : IUiScreen { }

	public class UiWinScreen : UiScreen, IUiWinScreen
	{
		public override Screen Screen => Screen.Win;
	}
}