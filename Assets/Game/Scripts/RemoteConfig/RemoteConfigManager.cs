namespace Game.RemoteConfig
{
	using Newtonsoft.Json;
	using System;
	using System.Collections.Generic;
	using UniRx;
	using Zenject;

	public interface IRemoteConfigManager
	{
		RemoteConfig RemoteConfig { get; }
		ReactiveCommand OnConfigReady { get; }
	}

	class RemoteConfigManager : IRemoteConfigManager, IInitializable, IDisposable
	{

		[Inject] IRemoteConfigLoader    _remoteConfigLoader;


		public const string ActualAppVersionKey					= "version";
		public const string BannedAppVersionsKey				= "bannedVersions";

		readonly CompositeDisposable _lifetimeDisposables = new CompositeDisposable();

		RemoteConfig _remoteConfig = new RemoteConfig();

		public void Initialize()
		{
			_remoteConfigLoader.State
				.Where( v => v != ERemoteConfigState.None )
				.Subscribe( DataReceivedHandler )
				.AddTo( _lifetimeDisposables );
		}

		public void Dispose() => _lifetimeDisposables.Clear();

#region IRemoteConfigManager

		public RemoteConfig RemoteConfig			=> _remoteConfig;

		public ReactiveCommand OnConfigReady		{ get; } = new ReactiveCommand();

#endregion

		private void DataReceivedHandler(ERemoteConfigState state)
		{
			if (state == ERemoteConfigState.Received)
				SetupConfig();

			OnConfigReady.Execute();
		}

		private void SetupConfig()
		{
			_remoteConfig.ActualAppVersion		= GetIntValue( ActualAppVersionKey );
			_remoteConfig.BannedAppVersions		= GetJsonValue<List<int>>( BannedAppVersionsKey );
		}

		private int GetIntValue( string key )
		{
			var value			= Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue( key );
			int returnValue		= (int)value.LongValue;

			Logger.Log( Logger.Module.RemoteConfig, $"Int parameter: {key} = {returnValue}" );

			return returnValue;
		}

		private T GetJsonValue<T>( string key )
		{
			var value			= Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue( key );
			string returnValue	= value.StringValue;

			Logger.Log( Logger.Module.RemoteConfig, $"Json parameter: {key} = {value.StringValue}" );
			
			return JsonConvert.DeserializeObject<T>( returnValue );
		}


		/*
		bool GetBoolValue(string name, bool defaultValue)
		{
			var defaultStringValue		= defaultValue? "1": "0";
			var value					= GameAnalytics.GetRemoteConfigsValueAsString( name, defaultStringValue );

			bool returnValue			= value == "1";

			Logger.Log( Logger.Module.RemoteConfig, $"Bool parameter: {name} = {returnValue}" );

			return returnValue;
		}
		*/
		/*
		float GetFloatValue(string name, float defaultValue)
		{
			CultureInfo ci				= CultureInfo.InvariantCulture;
			string value				= GameAnalytics.GetRemoteConfigsValueAsString( name, defaultValue.ToString(ci.NumberFormat) );

			float returnValue = (float.TryParse(value, NumberStyles.Float, ci.NumberFormat, out var result)) 
				? result
				: defaultValue;

			Logger.Log(EModule.RemoteConfig, $"Float parameter: {name} = {returnValue}");

			return returnValue;
		}

		T GetEnumValue<T>(string name, T defaultValue) where T : Enum
		{
			var value       = GameAnalytics.GetRemoteConfigsValueAsString( name, ((int)(object)defaultValue).ToString() );
			bool parsed     = int.TryParse( value, out var intValue ) || !Enum.IsDefined( typeof( T ), intValue );

			T returnValue = (!parsed) ? defaultValue : (T) (object) intValue;

			Logger.Log(EModule.RemoteConfig, $"{typeof(T)} parameter: {name} = {returnValue}");

			return returnValue;
		}
		*/
	}
}
