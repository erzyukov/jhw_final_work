namespace Game.Ui
{
	using Game.Configs;
	using Game.Core;
	using Game.Profiles;
	using System;
	using UniRx;
	using UnityEngine;
	using UnityEngine.UI;
	using Zenject;
	using TMPro;
	using DG.Tweening;

	public class EnergyElement : ProfileValueElement
	{
		[SerializeField] private Slider _energy;
		[SerializeField] private GameObject _restoreTimer;
		[SerializeField] private TextMeshProUGUI _restoreTimerValue;
		//[SerializeField] private GameObject _addonImage;
		[SerializeField] private Button _addonButton;

		[Header("Plus Alert Animation")]
		[SerializeField] private ImageAlertInput _addonImage;

		[Inject] private IGameEnergy _gameEnargy;
		[Inject] private GameProfile _gameProfile;
		[Inject] private EnergyConfig _energyConfig;
		[Inject] private IResourceEvents _resourceEvents;
		[Inject] private IUiRewardedEnergyWindow _rewardedWindow;

		private Tween _addonTween;
		private bool _isAddonPlaing;

		protected override void Subscribes()
		{
			_gameEnargy.EnergyRatio
				.Subscribe( OnEnergyRatioChanged )
				.AddTo( this );

			_gameProfile.Energy.Amount
				.Subscribe( OnEnergyValueChanged )
				.AddTo( this );

			_gameEnargy.SecondsToRestoreOnePoint
				.Subscribe( value => SetTimerValue( value ) )
				.AddTo( this );

			_resourceEvents.LowEnergyAlert
				.Subscribe( _ => PlayLowResourceAlert() )
				.AddTo( this );

			_addonButton.OnClickAsObservable()
				.Subscribe( _ => _rewardedWindow.SetActive( true ) )
				.AddTo( this );
		}

		private void OnEnergyRatioChanged( float value )
		{
			_energy.value = value;
			_restoreTimer.SetActive( value < 1 );
		}

		private void OnEnergyValueChanged( int value )
		{
			SetValue( $"{value}/{_energyConfig.MaxEnery}" );

			if (value < _energyConfig.LevelPrice && _isAddonPlaing == false)
			{
				_addonTween?.Rewind();
				_addonTween = _addonImage.Image.DOColor( _addonImage.Color, _addonImage.Duration )
					.SetLoops( _addonImage.Repeats, LoopType.Yoyo )
					.SetEase( Ease.InOutSine );
				_isAddonPlaing = true;
			}
			else if (value >= _energyConfig.LevelPrice && _isAddonPlaing)
			{
				_addonTween?.Rewind();
				_addonTween.Kill();
				_isAddonPlaing = false;
			}

			bool isEnaughtToBuff = value <= _energyConfig.MaxEnery - _energyConfig.LevelPrice;
			_addonButton.interactable = isEnaughtToBuff;
			_addonImage.Image.gameObject.SetActive( isEnaughtToBuff );
		}

		private void SetTimerValue( int value )
		{
			TimeSpan remainSeconds = TimeSpan.FromSeconds(value);
			_restoreTimerValue.text = remainSeconds.ToString( "mm\\:ss" );
		}
	}
}