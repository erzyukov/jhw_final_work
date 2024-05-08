namespace Game.Core
{
	using Game.Configs;
	using Game.Profiles;
	using Game.Units;
	using Game.Utilities;
	using UniRx;
	using Zenject;


	public interface IGameHeroSquad
	{
		void AddUnit( Species species );
		void RemoveUnit( Species species );
	}

	public class GameHeroSquad : ControllerBase, IGameHeroSquad, IInitializable
	{
		[Inject] private GameProfile			_profile;
		[Inject] private UnitsConfig			_unitsConfig;
		[Inject] private IGameProfileManager	_gameProfileManager;
		[Inject] private IGameUpgrades			_gameUpgrades;



		public void Initialize()
		{
			_gameUpgrades.Upgraded
				.Where( _ => _profile.Squad.Count < _unitsConfig.MaxSquadSize )
				.Subscribe( s => AddUnit( s ) )
				.AddTo( this );
		}

		public void AddUnit( Species species )
		{
			if (_profile.Squad.Contains( species ))
				return;

			_profile.Squad.Add( species );
			_gameProfileManager.Save();
		}

		public void RemoveUnit( Species species )
		{
			if (_profile.Squad.Contains( species ) == false)
				return;

			_profile.Squad.Remove( species );
			_gameProfileManager.Save();
		}
	}
}
