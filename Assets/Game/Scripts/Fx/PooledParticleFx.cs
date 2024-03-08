namespace Game.Fx
{
	using Zenject;

	public class PooledParticleFx : ParticleFx
	{
		Pool    _pool;

		public void Despawn()
		{
			_pool?.Despawn( this );
		}

		protected override void OnParticleSystemStopped()
		{
			base.OnParticleSystemStopped();

			Despawn();
		}

		public class Pool : MonoMemoryPool<PooledParticleFx>
		{
			[Inject] public VfxElement Type;

			protected override void OnCreated( PooledParticleFx item )
			{
				base.OnCreated( item );

				item._pool = this;
			}

			protected override void Reinitialize( PooledParticleFx item )
			{
				item.Play();
			}
		}
	}
}

