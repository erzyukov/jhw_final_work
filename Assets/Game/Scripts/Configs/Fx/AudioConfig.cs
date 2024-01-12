namespace Game.Configs
{
	using UnityEngine;
	using Game.Units;
	using System.Collections.Generic;
	using Sirenix.OdinInspector;

	[CreateAssetMenu(fileName = "Audio", menuName = "Configs/Audio", order = (int)Config.Audio)]
	public class AudioConfig : SerializedScriptableObject
	{
		[Header("Sound")]
		[SerializeField] private float _defaultSoundVolume;

		[Header("Units")]
		[SerializeField] private Dictionary<Species, AudioClip> _shoot = new Dictionary<Species, AudioClip>();

		public float DefaultSoundVolume => _defaultSoundVolume;

		public AudioClip GetShootClip(Species species)
		{
			if (_shoot.TryGetValue(species, out AudioClip value))
				return value;
			
			return null;
		}
	}
}
