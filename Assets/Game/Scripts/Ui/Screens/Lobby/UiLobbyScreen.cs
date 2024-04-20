namespace Game.Ui
{
	using UnityEngine;
	using TMPro;
	using UnityEngine.UI;
	using System;
	using UniRx;
	using Game.Utilities;
	using Sirenix.Utilities;
	using DG.Tweening;

	public interface IUiLobbyScreen : IUiScreen
	{
		IObservable<Unit> PlayButtonClicked { get; }
		IObservable<Unit> PreviousLevelClicked { get; }
		IObservable<Unit> NextLevelClicked { get; }
		void SetTitle( string value );
		void SetLastWaveValue( string value );
		void SetLastWaveActive( bool value );
		void SetPlayPriceText( string value );
		void SetPlayPriceActive( bool value );
		void SetPlayButtonEnabled( bool value );

		void SetLevelActive( bool value );
		void SetPrewLevelActive( bool value );
		void SetNextLevelActive( bool value );
		void SetLevelIcon( Sprite value );
		void SetPrewLevelIcon( Sprite value );
		void SetNextLevelIcon( Sprite value );
		void SetIconNormalizedPosition( float value );
		void SetPrevButtonActive( bool value );
		void SetPrevGlowActive( bool value );
		void SetNextButtonActive( bool value );
		void SetNextGlowActive( bool value );

		void SetNextLevelAnimationActive( bool value );
	}

	public class UiLobbyScreen : UiScreen, IUiLobbyScreen
	{
		[SerializeField] private Button _playButton;
		[SerializeField] private Image _playGlowImage;
		[SerializeField] private GameObject[] _playDecorates;
		[SerializeField] private CanvasGroup _playCanvasGroup;
		[SerializeField] private GameObject _playIcon;
		[SerializeField] private GameObject _playPrice;
		[SerializeField] private TextMeshProUGUI _playPriceText;

		[Header("LocationInfo")]
		[SerializeField] private TextMeshProUGUI _levelTitle;
		[SerializeField] private GameObject _lastWave;
		[SerializeField] private TextMeshProUGUI _lastWaveValue;
		[SerializeField] private Image _levelIcon;
		[SerializeField] private Image _prevLevelIcon;
		[SerializeField] private Image _nextLevelIcon;
		[SerializeField] private ScrollRect _levelScrollRect;
		[SerializeField] private Button _previousLevelButton;
		[SerializeField] private Button _nextLevelButton;
		[SerializeField] private GameObject _previousLevelGlow;
		[SerializeField] private GameObject _nextLevelGlow;
		[SerializeField] private GameObject _levelAlert;
		[SerializeField] private GameObject _prevLevelAlert;
		[SerializeField] private GameObject _nextLevelAlert;
		[SerializeField] private Image _levelIconGlow;
		[SerializeField] private Image _prevLevelIconGlow;
		[SerializeField] private Image _nextLevelIconGlow;

		[Header("Settings")]
		[SerializeField] private Material _regionDefaultMaterial;
		[SerializeField] private Material _regionDisabledMaterial;
		[SerializeField] private Sprite _activePlayGlowSprite;
		[SerializeField] private Sprite _disabledPlayGlowSprite;
		[SerializeField] private float _nextLevelAnimationScale;
		[SerializeField] private float _nextLevelAnimationDuration;

		public override Screen Screen => Screen.Lobby;

		private Tween _nextButtonAnimation;

		#region IUiLobbyScreen

		public IObservable<Unit> PlayButtonClicked => _playButton.OnClickAsObservable();
		public IObservable<Unit> PreviousLevelClicked => _previousLevelButton.OnClickAsObservable();
		public IObservable<Unit> NextLevelClicked => _nextLevelButton.OnClickAsObservable();

		public void SetTitle( string value ) => _levelTitle.text = value;

		public void SetLastWaveValue( string value ) => _lastWaveValue.text = value;

		public void SetLastWaveActive( bool value ) => _lastWave.SetActive( value );

		public void SetPlayPriceActive( bool value )
		{
			_playIcon.SetActive( !value );
			_playPrice.SetActive( value );
		}

		public void SetPlayPriceText( string value ) => _playPriceText.text = value;

		public void SetPlayButtonEnabled( bool value )
		{
			_playButton.interactable = value;
			_playGlowImage.sprite = value ? _activePlayGlowSprite : _disabledPlayGlowSprite;
			_playCanvasGroup.alpha = GetAlphaByActive( value );
			_playDecorates.ForEach( e => e.SetActive( value ) );
		}

		public void SetLevelIcon( Sprite value ) => _levelIcon.sprite = value;
		public void SetPrewLevelIcon( Sprite value ) => _prevLevelIcon.sprite = value;
		public void SetNextLevelIcon( Sprite value ) => _nextLevelIcon.sprite = value;
		public void SetIconNormalizedPosition( float value ) => 
			_levelScrollRect.horizontalNormalizedPosition = value;

		public void SetPrevButtonActive( bool value ) => _previousLevelButton.interactable = value;
		public void SetNextButtonActive( bool value ) => _nextLevelButton.interactable = value;

		public void SetPrevGlowActive( bool value ) => _previousLevelGlow.SetActive( value );
		public void SetNextGlowActive( bool value ) => _nextLevelGlow.SetActive( value );

		public void SetLevelActive( bool value ) =>
			SetRegionActive( value, _levelAlert, _levelIcon, _levelIconGlow );

		public void SetPrewLevelActive( bool value ) =>
			SetRegionActive( value, _prevLevelAlert, _prevLevelIcon, _prevLevelIconGlow );

		public void SetNextLevelActive( bool value ) =>
			SetRegionActive( value, _nextLevelAlert, _nextLevelIcon, _nextLevelIconGlow );

		public void SetNextLevelAnimationActive( bool value )
		{
			if (value == false)
			{
				_nextButtonAnimation?.Rewind();
				_nextButtonAnimation?.Kill();
			}
			else
			{
				_nextButtonAnimation = _nextLevelButton.transform.DOScale( _nextLevelAnimationScale, _nextLevelAnimationDuration )
					.SetEase( Ease.InOutSine )
					.SetLoops( -1, LoopType.Yoyo );
			}
		}

		#endregion

		private void SetRegionActive( bool value, GameObject alert, Image icon, Image glow )
		{
			alert.SetActive( !value );
			float alpha = GetAlphaByActive( value );
			icon.color = icon.color.WithAlpha( alpha );
			glow.color = glow.color.WithAlpha( alpha );
			
			icon.material = value ? _regionDefaultMaterial : _regionDisabledMaterial;
		}

		private float GetAlphaByActive( bool value ) => value ? 1f : 0.5f;
	}
}
