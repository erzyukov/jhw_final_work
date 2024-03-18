namespace Game.Ui
{
	using System;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;

	public interface IUiRewardedButton
	{
		ERewardedType Type { get; }
		IObservable<Unit> Clicked { get; }
	}

	public class UiRewardedButton : MonoBehaviour, IUiRewardedButton
	{
		[SerializeField] private Button _button;
		[SerializeField] private ERewardedType _type;

		#region IUiRewardedButton

		public ERewardedType Type => _type;

		public IObservable<Unit> Clicked => _button.OnClickAsObservable();

		#endregion

	}
}
