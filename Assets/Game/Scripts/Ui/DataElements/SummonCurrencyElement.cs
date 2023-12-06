namespace Game.Ui
{
	using UniRx;

	public class SummonCurrencyElement : ProfileValueElement
	{
		protected override void Subscribes()
		{
			Profile.SummonCurrency
				.Subscribe(SetValue)
				.AddTo(this);
		}
	}
}