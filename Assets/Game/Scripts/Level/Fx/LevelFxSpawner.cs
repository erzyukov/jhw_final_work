namespace Game.Level
{
	using Game.Core;
	using Game.Fx;
	using Game.Utilities;
	using Zenject;
	using UniRx;

	public class LevelFxSpawner : ControllerBase, IInitializable
	{
		[Inject] private IEffectsSpawner _effectsSpawner;
		[Inject] private IBattleEvents _battleEvents;

		public void Initialize()
		{
			_battleEvents.DamageApplyed
				.Where( d => d.Type == EDamageType.Aoe )
				.Subscribe( d => _effectsSpawner.Spawn( VfxElement.GrenadeExplode, d.Position ) )
				.AddTo( this );
		}
	}
}
