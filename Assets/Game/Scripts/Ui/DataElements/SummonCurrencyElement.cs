namespace Game.Ui
{
	using UniRx;
	using UnityEngine;

	public class SummonCurrencyElement : ProfileValueElement
	{
		[SerializeField] private string _valuePostfix;

		protected override void Subscribes()
		{
			Profile.SummonCurrency
				.Subscribe(v => SetValue(v.ToString() + _valuePostfix))
				.AddTo(this);
		}
	}
}