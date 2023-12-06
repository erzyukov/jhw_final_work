namespace Game.Ui
{
	using UniRx;

	public class SoftCurrencyElement : ProfileValueElement
	{
		protected override void Subscribes()
		{
			Profile.SoftCurrency
				.Subscribe(SetValue)
				.AddTo(this);
		}
	}
}