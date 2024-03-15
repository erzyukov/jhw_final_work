namespace Game.Utilities
{
	using UnityEngine;
	using UnityEngine.UI;
	using Zenject;
	using UniRx;
	using Game.Core;
	using Game.Configs;

	public class LoadingScreen : MonoBehaviour
    {
		[SerializeField] private Image _loadingText;

		[Inject] private ILocalizator _localizator;
		[Inject] private LocalizationConfig _config;

        void Awake()
        {
			if (_localizator.LangKey.Value != null)
			{
				SetupLoadingText( _localizator.LangKey.Value );
			}
			else
			{
				_localizator.LangKey
					.Where( v => v != null )
					.Subscribe( key => SetupLoadingText( key ) )
					.AddTo( this );
			}
        }

		private void SetupLoadingText( string key )
		{
			_loadingText.sprite = _config.GetLoadingSprite( key );
			_loadingText.enabled = true;
		}
	}
}
