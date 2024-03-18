namespace Game.Ui
{
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public interface IUiRewardedEnergyWindow : IUiWindow { }

	public class UiRewardedEnergyWindow : UiWindow, IUiRewardedEnergyWindow
	{
		[SerializeField] private Button _adButton;

		protected override void Start()
		{
			base.Start();

			_adButton.OnClickAsObservable()
				.Subscribe( _ => SetActive( false ) )
				.AddTo( this );
		}

		#region IUiRewardedEnergyWindow

		public override Window Type => Window.RewardedEnergy;

		#endregion
	}
}