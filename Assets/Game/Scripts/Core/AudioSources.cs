namespace Game.Core
{
	using UnityEngine;

	public interface IAudioSources
	{
		AudioSource Sound { get; }
	}

    public class AudioSources : MonoBehaviour, IAudioSources
	{
		[SerializeField] AudioSource _sound;

		#region IAudioSources

		public AudioSource Sound => _sound;

		#endregion

	}
}
