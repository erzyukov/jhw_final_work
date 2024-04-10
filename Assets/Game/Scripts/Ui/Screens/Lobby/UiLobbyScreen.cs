namespace Game.Ui
{
	using UnityEngine;
	using TMPro;
	using UnityEngine.UI;
	using System;
	using UniRx;

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

		void SetLevelIcon( Sprite value );
		void SetPrewLevelIcon( Sprite value );
		void SetNextLevelIcon( Sprite value );
		void SetIconNormalizedPosition( float value );
		void SetPrewButtonActive( bool value );
		void SetNextButtonActive( bool value );
	}

	public class UiLobbyScreen : UiScreen, IUiLobbyScreen
	{
		[SerializeField] private Button _playButton;
		[SerializeField] private GameObject _playIcon;
		[SerializeField] private GameObject _playPrice;
		[SerializeField] private TextMeshProUGUI _playPriceText;

		[Header("LocationInfo")]
		[SerializeField] private TextMeshProUGUI _levelTitle;
		[SerializeField] private GameObject _lastWave;
		[SerializeField] private TextMeshProUGUI _lastWaveValue;
		[SerializeField] private Image _levelIcon;
		[SerializeField] private Image _prewLevelIcon;
		[SerializeField] private Image _nextLevelIcon;
		[SerializeField] private ScrollRect _levelScrollRect;
		[SerializeField] private Button _previousLevelButton;
		[SerializeField] private Button _nextLevelButton;

		public override Screen Screen => Screen.Lobby;

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

		public void SetPlayButtonEnabled( bool value ) => _playButton.interactable = value;

		public void SetLevelIcon( Sprite value ) => _levelIcon.sprite = value;
		public void SetPrewLevelIcon( Sprite value ) => _prewLevelIcon.sprite = value;
		public void SetNextLevelIcon( Sprite value ) => _nextLevelIcon.sprite = value;
		public void SetIconNormalizedPosition( float value ) => 
			_levelScrollRect.horizontalNormalizedPosition = value;

		public void SetPrewButtonActive( bool value ) => _previousLevelButton.interactable = value;
		public void SetNextButtonActive( bool value ) => _nextLevelButton.interactable = value;

		#endregion
	}
}
