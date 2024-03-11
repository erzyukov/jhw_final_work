namespace Game.Utilities
{
	using Game.Profiles;
	using UnityEngine;
	using UnityEngine.UI;
	using Zenject;
	using UniRx;
	using Game.Core;
	using System;

	public class YandexLoadingScreen : MonoBehaviour
    {
		[SerializeField] private Image _loadingText;

		[Inject] private IGameProfileManager _profileManager;
		[Inject] IScenesManager _scenesManager;
		[Inject] private ILocalizator _localizator;

		private const string LoadingTextKey = "loadingText";

        void Awake()
        {
			if (_localizator.IsLangInitialized.Value)
			{
				SetupLoadingText();
			}
			else
			{
				_localizator.IsLangInitialized
					.Where( v => v )
					.Subscribe( _ => SetupLoadingText() )
					.AddTo( this );
			}
        }

		private void SetupLoadingText()
		{
			_loadingText.sprite = _localizator.GetSprite(LoadingTextKey);
			_loadingText.enabled = true;
		}
	}
}
