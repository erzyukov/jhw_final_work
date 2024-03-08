namespace Game.Fx
{
	using System.Collections.Generic;
	using UnityEngine;
	using Zenject;

	public interface IEffectsSpawner
	{
		PooledParticleFx Spawn( VfxElement type, Vector3 position, Transform parent );
		PooledParticleFx Spawn( VfxElement type, Vector3 position );
		void Despawn( PooledParticleFx fx );
	}

	public class EffectsSpawner : IEffectsSpawner
	{
		[Inject] private List<PooledParticleFx.Pool> _pools;

		public PooledParticleFx Spawn( VfxElement type, Vector3 position, Transform parent = null )
		{
			if (type == VfxElement.None)
				return null;

			PooledParticleFx.Pool pool = _pools.Find( p => p.Type == type );

			if (pool == null)
			{
				Debug.LogWarning( $"Particle pool with id {type} not found" );

				return null;
			}

			PooledParticleFx particle = pool.Spawn();

			// Transform
			Transform tr = particle.transform;
			tr.SetParent( parent );

			if (parent != null)
				tr.rotation = parent.rotation;

			return particle;
		}

		public PooledParticleFx Spawn( VfxElement type, Vector3 position )
		{
			return Spawn( type, position, null );
		}

		public void Despawn( PooledParticleFx fx )
		{
			if (fx != null)
				fx.Despawn();
		}
	}
}
