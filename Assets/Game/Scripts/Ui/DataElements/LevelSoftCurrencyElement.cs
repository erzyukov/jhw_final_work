namespace Game.Ui
{
	using UniRx;

	public class LevelSoftCurrencyElement : ProfileValueElement
	{
		protected override void Subscribes()
		{
			Profile.LevelSoftCurrency
				.Subscribe(SetValue)
				.AddTo(this);
		}
	}
}