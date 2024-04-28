namespace Game.Profiles
{
	using Game.Configs;
	using Game.Units;
	using Game.Utilities;
	using System;
	using System.Collections.Generic;
	using UniRx;
	using Zenject;

	public interface IGameProfileManager
	{
		BoolReactiveProperty IsReady { get; }
		GameProfile GameProfile { get; }
		void Save( bool forceInstant = false );
		void Reset();
	}

	public class GameProfileManager : ControllerBase, IGameProfileManager
	{
		[Inject] private LevelsConfig _levelsConfig;
		[Inject] private UnitsConfig _unitsConfig;
		[Inject] private EnergyConfig _energyConfig;
		[Inject] private IProfileSaver _profileSaver;

		private const float SaveDelay = 2;

		private GameProfile _gameProfile;
		private ITimer _timer = new Timer(true);
		private bool _delayedSave;

		public void OnInstantiated()
		{
			CreateGameProfile();

			_profileSaver.SaveSystemReady
				.Subscribe( _ => OnSaveSystemReady() )
				.AddTo( this );
		}

		#region IGameProfileManager

		public BoolReactiveProperty IsReady { get; } = new BoolReactiveProperty();

		public GameProfile GameProfile => _gameProfile;

		public void Save( bool forceInstant = false )
		{
			if (forceInstant)
			{
				_profileSaver.Save( _gameProfile );
				_timer.Set( SaveDelay );
				return;
			}

			if (_delayedSave)
				return;

			_delayedSave = true;

			Observable
				.Timer( TimeSpan.FromSeconds( _timer.Remained ) )
				.Subscribe( _ =>
				{
					_profileSaver.Save( _gameProfile );
					_timer.Set( SaveDelay );
					_delayedSave = false;
				} )
				.AddTo( this );
		}

		public void Reset()
		{
			CreateGameProfile();
			Save();
		}

		#endregion

		private void CreateGameProfile()
		{
			_gameProfile = new GameProfile();
			FillUnits();
			_gameProfile.Energy.Amount.Value = _energyConfig.MaxEnery;
		}

		private void OnSaveSystemReady()
		{
			if (_profileSaver.Load( out GameProfile loadedProfile ))
				_gameProfile = loadedProfile;

			AddMissing();
			IsReady.Value = true;
		}

		private void AddMissing()
		{
			AddMissingLevels();
			AddMissingUnits();
			AddMissingEnergy();
			AddMissingAnalytics();
			AddMissingIapShop();

			Save();
		}

		private void AddMissingIapShop()
		{
			/*
			if (_gameProfile.IapShopProfile == null)
				_gameProfile.IapShopProfile = new();

			if (_gameProfile.IapShopProfile.BoughtProducts == null)
				_gameProfile.IapShopProfile.BoughtProducts = new();

			if (_gameProfile.IapShopProfile.NoAdsProduct == null)
				_gameProfile.IapShopProfile.NoAdsProduct = new();
			*/
		}

		private void AddMissingLevels()
		{
			if (_gameProfile.Levels == null)
				_gameProfile.Levels = new List<LevelProfile>();

			while (_gameProfile.Levels.Count < _levelsConfig.Levels.Length)
			{
				bool isUnlocked = _gameProfile.Levels.Count < _gameProfile.LevelNumber.Value;
				_gameProfile.Levels.Add( new LevelProfile() );
				_gameProfile.Levels[ _gameProfile.Levels.Count - 1 ].Unlocked.Value = isUnlocked;
			}
		}

		private void AddMissingUnits()
		{
			if (_gameProfile.Units == null)
				_gameProfile.Units = new UnitsProfile();

			if (_gameProfile.Units.Upgrades == null)
				_gameProfile.Units.Upgrades = new Dictionary<Species, IntReactiveProperty>();

			FillUnits();
		}

		private void FillUnits()
		{
			foreach (var species in _unitsConfig.HeroUnits)
			{
				if (_gameProfile.Units.Upgrades.ContainsKey( species ) == false)
				{
					int defaultLevel = _unitsConfig.HeroDefaultSquad.Contains( species ) ? 1 : 0 ;
					_gameProfile.Units.Upgrades.Add( species, new IntReactiveProperty( defaultLevel ) );
				}
			}
		}

		private void AddMissingEnergy()
		{
			if (_gameProfile.Energy == null)
				_gameProfile.Energy = new EnergyProfile();

			if (_gameProfile.Energy.LastEnergyChange == new DateTime())
				_gameProfile.Energy.LastEnergyChange = DateTime.Now.Subtract( TimeSpan.FromDays( 1 ) );
		}

		private void AddMissingAnalytics()
		{
			if (_gameProfile.Analytics == null)
				_gameProfile.Analytics = new AnalyticsProfile();
		}
	}
}