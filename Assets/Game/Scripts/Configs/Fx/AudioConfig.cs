namespace Game.Configs
{
	using UnityEngine;
	using Game.Units;
	using System.Collections.Generic;
	using Sirenix.OdinInspector;
	using Game.Core;
	using System;

	[CreateAssetMenu(fileName = "Audio", menuName = "Configs/Audio", order = (int)Config.Audio)]
	public class AudioConfig : SerializedScriptableObject
	{
		[Header("UI")]
		[SerializeField] private float _defaultUiVolume;
		[SerializeField] private AudioClip _uiButtonClick;

		[Header("Music")]
		[SerializeField] private float _defaultMusicVolume;
		[SerializeField] private float _fadeDuration;

		[Header("Sound")]
		[SerializeField] private float _defaultSoundVolume;

		[Header("Units")]
		[SerializeField] private Dictionary<Species, AudioClip> _shoot = new Dictionary<Species, AudioClip>();
		
		[Header("Game")]
		[SerializeField] private Dictionary<GameState, GameStateAudioClip> _gameStateThemes = new Dictionary<GameState, GameStateAudioClip>();

		public float DefaultUiVolume => _defaultUiVolume;
		public AudioClip UiButtonClick => _uiButtonClick;
		public float DefaultSoundVolume => _defaultSoundVolume;
		public float DefaultMusicVolume => _defaultMusicVolume;
		public float FadeDuration => _fadeDuration;

		public AudioClip GetShootClip(Species species)
		{
			if (_shoot.TryGetValue(species, out AudioClip value))
				return value;
			
			return null;
		}

		public AudioClip GetGameStateTheme(GameState state, out float volume)
		{
			if (_gameStateThemes.TryGetValue(state, out GameStateAudioClip value))
			{
				volume = value.Volume;
				return value.AudioClip;
			}

			volume = 0;
			return null;
		}

		[Serializable]
		public struct GameStateAudioClip
		{
			public float Volume;
			public AudioClip AudioClip;
		}
	}
}
