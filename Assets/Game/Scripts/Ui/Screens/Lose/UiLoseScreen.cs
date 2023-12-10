namespace Game.Ui
{
	public interface IUiLoseScreen : IUiScreen { }

	public class UiLoseScreen : UiScreen, IUiLoseScreen
	{
		public override Screen Screen => Screen.Lose;
	}
}