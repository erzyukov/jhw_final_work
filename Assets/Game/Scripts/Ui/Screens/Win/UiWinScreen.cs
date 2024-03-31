namespace Game.Ui
{
	using Game.Utilities;
	using System;
	using TMPro;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public interface IUiWinScreen : IUiScreen
	{
		IObservable<Unit> RewardedClicked { get; }
		void SetSliderValue( float ratio );
		void SetLevelRewardValue( int value );
		void SetAdRewardValue( int value );
		void SetSelectLightAlpha( float value );
		void SetSkipButtonInteractable( bool value );
		void SetCloseButtonInteractable( bool value );
		void SetRequestParentActive( bool value );
		void SetCompleteParentActive( bool value );
		void SetAdRewardActive( bool value );
	}

	public class UiWinScreen : UiScreen, IUiWinScreen
	{
		[SerializeField] private Slider _multiplierSlider;
		[SerializeField] private TextMeshProUGUI _levelRewardText;
		[SerializeField] private TextMeshProUGUI _rewardedText;
		[SerializeField] private Button _skipButton;
		[SerializeField] private Button _rewardedButton;
		[SerializeField] private Button _windowCloseButton;
		[SerializeField] private Image _selectLight;
		[SerializeField] private GameObject _requestParent;
		[SerializeField] private GameObject _completeParent;

		public override Screen Screen => Screen.Win;
		
		protected override void Start()
		{
			_skipButton.OnClickAsObservable()
				.Subscribe(_ => OnCloseHandler())
				.AddTo(this);

			base.Start();
		}

		#region IUiWinScreen

		public IObservable<Unit> RewardedClicked => _rewardedButton.OnClickAsObservable();

		public void SetSliderValue( float ratio ) =>
			_multiplierSlider.value = ratio;

		public void SetLevelRewardValue( int value ) =>
			_levelRewardText.text = $"+ {value}";

		public void SetAdRewardValue( int value ) =>
			_rewardedText.text = $"+ {value}";

		public void SetSkipButtonInteractable( bool value ) =>
			_skipButton.interactable = value;

		public void SetCloseButtonInteractable( bool value ) =>
			_windowCloseButton.interactable = value;

		public void SetRequestParentActive( bool value ) =>
			_requestParent.SetActive( value );

		public void SetCompleteParentActive( bool value ) =>
			_completeParent.SetActive( value );

		public void SetSelectLightAlpha( float value ) =>
			_selectLight.SetAlpha( value );

		public void SetAdRewardActive( bool value ) => 
			_rewardedText.gameObject.SetActive( value );

		#endregion

	}
}