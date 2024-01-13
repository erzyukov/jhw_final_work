namespace Game.Core
{
	using UnityEngine;

	public interface IAudioSources
	{
		AudioSource Sound { get; }
		AudioSource Music { get; }
	}

    public class AudioSources : MonoBehaviour, IAudioSources
	{
		[SerializeField] AudioSource _sound;
		[SerializeField] AudioSource _music;

		#region IAudioSources

		public AudioSource Sound => _sound;
		
		public AudioSource Music => _music;

		#endregion

	}
}
