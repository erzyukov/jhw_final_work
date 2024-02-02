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
		[SerializeField] private AudioSource _sound;
		[SerializeField] private AudioSource _music;
		[SerializeField] private AudioSource _ui;

		#region IAudioSources

		public AudioSource Sound => _sound;
		
		public AudioSource Music => _music;
		
		public AudioSource Ui => _ui;

		#endregion

	}
}
