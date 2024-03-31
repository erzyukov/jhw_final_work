namespace Game.Utilities
{
	using Game.Core;
	using Zenject;
	using UniRx;
	using YG;

	public class YandexGameHandler : ControllerBase, IInitializable
	{
		[Inject] private IScenesManager _scenesManager;
		[Inject] private ILocalizator _localizator;

		public void Initialize()
		{
			_scenesManager.ResourceLoading
				.Subscribe(_ => InitLang())
				.AddTo( this );

			_scenesManager.MainLoaded
				.Subscribe(_ => OnMainLoaded())
				.AddTo( this );
		}

		private void InitLang()
		{
			YandexGame.LanguageRequest();
			_localizator.SetLocale( YandexGame.lang );
		}

		private void OnMainLoaded()
		{
			YandexGame.GameReadyAPI();
		}
	}
}
