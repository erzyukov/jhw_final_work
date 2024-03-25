namespace Game.Ui
{
	using Game.Profiles;
	using Game.Utilities;
	using Zenject;

	public class UiIapShopPresenter : ControllerBase, IInitializable
	{
		[Inject] private IUiIapShopScreen _screen;
		[Inject] private GameProfile _gameProfile;

		public void Initialize()
		{
			
		}

	}
}
