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

	public class EnergyElement : ProfileValueElement
	{
		[SerializeField] private Slider _energy;
		[SerializeField] private GameObject _restoreTimer;
		[SerializeField] private TextMeshProUGUI _restoreTimerValue;
		[SerializeField] private GameObject _addonImage;
		[SerializeField] private Button _addonButton;

		[Inject] private IGameEnergy _gameEnargy;
		[Inject] private GameProfile _gameProfile;
		[Inject] private EnergyConfig _energyConfig;
		[Inject] private IResourceEvents _resourceEvents;

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
		}

		private void OnEnergyRatioChanged( float value )
		{
			_energy.value = value;
			_restoreTimer.SetActive( value < 1 );
		}

		private void OnEnergyValueChanged( int value )
		{
			SetValue( $"{value}/{_energyConfig.MaxEnery}" );

			_addonImage.SetActive( value < _energyConfig.LevelPrice );
			_addonButton.interactable = value <= _energyConfig.MaxEnery - _energyConfig.LevelPrice;
		}

		private void SetTimerValue( int value )
		{
			TimeSpan remainSeconds = TimeSpan.FromSeconds(value);
			_restoreTimerValue.text = remainSeconds.ToString( "mm\\:ss" );
		}
	}
}