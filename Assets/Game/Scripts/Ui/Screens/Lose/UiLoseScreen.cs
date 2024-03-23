namespace Game.Ui
{
	using UnityEngine;
	using UnityEngine.UI;
	using UniRx;
	using TMPro;

	public interface IUiLoseScreen : IUiScreen
	{
		void SetDefaulBlockActive(bool value);
		void SetRewardedBlockActive(bool value);
		void SetBonusTokenAmount(int amount);
		void SetCloseButtonInteractable( bool value );
	}

	public class UiLoseScreen : UiScreen, IUiLoseScreen
	{
		[SerializeField] private GameObject _defaultBlock;
		[SerializeField] private GameObject _rewardedBlock;
		[SerializeField] private Button _skipButton;
		[SerializeField] private TextMeshProUGUI _tokenAmountText;

		public override Screen Screen => Screen.Lose;

		protected override void Start()
		{
			_skipButton.OnClickAsObservable()
				.Subscribe(_ => OnCloseHandler())
				.AddTo(this);

			base.Start();
		}

		#region IUiLoseScreen

		public void SetDefaulBlockActive( bool value ) =>
			_defaultBlock.SetActive( value );

		public void SetRewardedBlockActive( bool value ) =>
			_rewardedBlock.SetActive( value );

		public void SetBonusTokenAmount( int value ) =>
			_tokenAmountText.text = $"+ {value}";

		public void SetCloseButtonInteractable( bool value ) =>
			CloseButton.interactable = value;

		#endregion
	}
}