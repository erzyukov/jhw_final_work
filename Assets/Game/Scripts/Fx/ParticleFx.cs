namespace Game.Fx
{
	using UnityEngine;

    public class ParticleFx : MonoBehaviour
    {
		[SerializeField] private ParticleSystem _particleSystem;

		private bool _isPaused;

		public void Play()
		{
			_particleSystem.Play(true);
		}

		public void Play(bool toPlay)
		{
			if (toPlay)
			{
				_particleSystem.Play();
			}
			else
			{
				_isPaused = false;
				_particleSystem.Stop();
			}
		}

		public void Pause()
		{
			if (_particleSystem.isPlaying)
			{
				_isPaused = true;
				_particleSystem.Pause();
			}
		}

		public void Unpause()
		{
			if (_isPaused)
			{
				_isPaused = false;
				_particleSystem.Play();
			}
		}

		protected virtual void OnParticleSystemStopped() {}
	}
}
