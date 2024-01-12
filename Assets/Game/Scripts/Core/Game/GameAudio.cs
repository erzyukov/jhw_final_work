namespace Game.Core
{
	using Game.Configs;
	using Game.Units;
	using Game.Utilities;
	using UnityEngine;
	using Zenject;

	public interface IGameAudio
	{
		void PlayUnitShoot(Species species);
	}

	public class GameAudio : ControllerBase, IGameAudio, IInitializable
	{
		[Inject] private IAudioSources _audioSources;
		[Inject] private AudioConfig _audioConfig;

		public void Initialize()
		{
			_audioSources.Sound.volume = _audioConfig.DefaultSoundVolume;
		}

		#region IGameAudio

		public void PlayUnitShoot(Species species)
		{
			AudioClip clip = _audioConfig.GetShootClip(species);

			if (clip != null)
				_audioSources.Sound.PlayOneShot(clip);
		}

		#endregion
	}
}
