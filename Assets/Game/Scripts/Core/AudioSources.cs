namespace Game.Core
{
	using UnityEngine;

	public interface IAudioSources
	{
		AudioSource Sound { get; }
		AudioSource Music { get; }
		AudioSource Ui { get; }
	}

    public class AudioSources : MonoBehaviour, IAudioSources
	{
		[SerializeField] AudioSource _sound;
		[SerializeField] AudioSource _music;
		[SerializeField] AudioSource _ui;

		#region IAudioSources

		public AudioSource Sound => _sound;
		
		public AudioSource Music => _music;
		
		public AudioSource Ui => _ui;

		#endregion

	}
}
