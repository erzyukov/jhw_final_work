namespace Game.Ui
{
	using Game.Utilities;
	using Game.Profiles;
	using Zenject;
	using UniRx;

	public class UiCommonGameplay : ControllerBase, IInitializable
	{
		[Inject] private IUiCommonGameplayHud _hud;
		[Inject] private GameProfile _gameProfile;

		public void Initialize()
		{
			_gameProfile.SoftCurrency
				.Subscribe(_hud.SetSoftCurrencyAmount)
				.AddTo(this);

			_gameProfile.SummonCurrency
				.Subscribe(_hud.SetSummonCurrencyAmount)
				.AddTo(this);

			_hud.Opening
				.Subscribe(_ => OnOpeningHandler())
				.AddTo(this);
		}

		private void OnOpeningHandler()
		{
			// TODO: Set wave value
		}
	}
}
