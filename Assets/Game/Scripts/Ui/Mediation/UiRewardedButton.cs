namespace Game.Ui
{
	using System;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;
	using Zenject;
	using Game.Managers;
	using TMPro;
	using Sirenix.Utilities;
	using Game.Utilities;

	public interface IUiRewardedButton
	{
		ERewardedType Type { get; }
		IObservable<Unit> Clicked { get; }
	}

	public class UiRewardedButton : MonoBehaviour, IUiRewardedButton
	{
		[Inject] IAdsManager	_adsManager;

		[SerializeField] private Button				_button;
		[SerializeField] private ERewardedType		_type;
		[SerializeField] private GameObject			_preloader;
		[SerializeField] private Image[]            _imageDecors;
		[SerializeField] private TextMeshProUGUI[]  _textDecors;

		private const float		DisabledAlpha = 0.5f;

		private void Awake()
		{
			_adsManager.IsRewardedAvailable
				.Subscribe( SetAdLoadingState )
				.AddTo( this );
		}

#region IUiRewardedButton

		public ERewardedType		Type		=> _type;

		public IObservable<Unit>	Clicked		=> _button.OnClickAsObservable();

#endregion

		private void SetAdLoadingState( bool isAvailable )
		{
			_imageDecors.ForEach( e => 
				e.color = isAvailable? e.color.WithAlpha( 1 ) : e.color.WithAlpha( DisabledAlpha )
			);
			_textDecors.ForEach( e => 
				e.color = isAvailable? e.color.WithAlpha( 1 ) : e.color.WithAlpha( DisabledAlpha )
			);
			
			_preloader.SetActive( !isAvailable );
			_button.interactable	= isAvailable;
		}

	}
}
