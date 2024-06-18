namespace Game.Analytics
{
	using Zenject;
	using UniRx;
	using Game.Profiles;
	using Game.Utilities;
	using System.Collections.Generic;
	using UnityEngine;

	public class SettingsAnalytics : AnalyticsBase, IInitializable
	{
		[Inject] private GameProfile	_gameProfile;

		private const string	MusicSettingsEventKey	= "settings_music";
		private const string	SoundSettingsEventKey	= "settings_sound";

		private const string	OnValue		= "on";
		private const string	OffValue	= "off";

		public void Initialize()
		{
			_gameProfile.IsMusicEnabled
				.Skip( 1 )
				.Subscribe( v => OnToggleSettingsChanged( MusicSettingsEventKey, v ) )
				.AddTo( this );

			_gameProfile.IsSoundEnabled
				.Skip( 1 )
				.Subscribe( v => OnToggleSettingsChanged( SoundSettingsEventKey, v ) )
				.AddTo( this );
		}

		private void OnToggleSettingsChanged( string key, bool value )
		{
			var properties = new Dictionary<string, object>
			{
				{ "value", GetToggleString(value) },
				{ "time", Time.time },
			};
			SendMessage( key, properties );
		}

		private string GetToggleString(bool value) =>
			value ? OnValue : OffValue;
	}
}
